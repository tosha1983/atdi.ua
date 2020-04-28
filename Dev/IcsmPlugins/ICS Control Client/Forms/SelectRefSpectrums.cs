using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XICSM.ICSControlClient.Models.Views;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using FM = XICSM.ICSControlClient.Forms;
using Microsoft.VisualBasic.FileIO;
using ICSM;
using Atdi.Common;

namespace XICSM.ICSControlClient.Forms
{
    public partial class SelectRefSpectrums : Form
    {
        private IList _currentShortSensor;
        public Dictionary<int, SDR.ReferenceSituation> listRef;
        public bool IsPresOK = false;

        public SelectRefSpectrums(IList currentShortSensor)
        {
            InitializeComponent();
            _currentShortSensor = currentShortSensor;
        }

        private void form_load(object sender, EventArgs e)
        {
            panel.Width = this.Width - 5;
            panel.Height = this.Height - 80;
            cmdOk.Text = "Ok";
            cmdCancel.Text = Properties.Resources.Cancel;
            int i = 0;
            listRef = new Dictionary<int, SDR.ReferenceSituation>();

            foreach (ShortSensorViewModel shortSensor in this._currentShortSensor)
            {
                var label = new Label(){ Left = 5, Top = i * 30, Width = 240 };
                label.Text = string.IsNullOrEmpty(shortSensor.Title) ? shortSensor.Name : shortSensor.Title;
                panel.Controls.Add(label);

                var button = new Button() { Name = $"button{i}", Left = 250, Top = i * 30, Width = 90, Text = Properties.Resources.Download, Tag = i };
                button.Click += DownloadClick;
                panel.Controls.Add(button);

                var textBox = new TextBox() { Name = $"path{i}", Left = 350, Top = i * 30, Tag = i, Width = 500 };
                panel.Controls.Add(textBox);
                i++;
            }
        }
        private void DownloadClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            int.TryParse(button.Tag.ToString(), out int sensorIndex);
            var refSit = DownloadRefSituation(sensorIndex);

            if (listRef.ContainsKey(sensorIndex))
                listRef[sensorIndex] = refSit;
            else
                listRef.Add(sensorIndex, refSit);
        }

        private SDR.ReferenceSituation DownloadRefSituation(int sensorIndex)
        {
            ShortSensorViewModel shortSensor = this._currentShortSensor[sensorIndex] as ShortSensorViewModel;
            string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            SDR.ReferenceSituation refSit = new SDR.ReferenceSituation();

            OpenFileDialog openFile = new OpenFileDialog() { Filter = "Текстовые файлы(*.csv)|*.csv", Title = shortSensor.Name };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                var _waitForm = new FM.WaitForm();
                _waitForm.SetMessage("Loading file. Please wait...");
                _waitForm.TopMost = true;
                _waitForm.Show();
                _waitForm.Refresh();

                List<SDR.ReferenceSignal> listRefSig = new List<SDR.ReferenceSignal>();

                var svcSensor = SVC.SdrnsControllerWcfClient.GetSensorById(shortSensor.Id);
                SDR.SensorLocation sensorLocation = null;
                if (svcSensor.Locations != null && svcSensor.Locations.Length > 0)
                    sensorLocation = svcSensor.Locations[svcSensor.Locations.Length - 1];

                using (TextFieldParser parser = new TextFieldParser(openFile.FileName))
                {
                    int i = 0;
                    try
                    {
                        parser.TextFieldType = FieldType.Delimited;
                        parser.SetDelimiters(";");
                        while (!parser.EndOfData)
                        {
                            i++;
                            var record = parser.ReadFields();

                            if (i >= 4)
                            {
                                SDR.ReferenceSignal refSig = new SDR.ReferenceSignal();

                                double? f = record[9].Replace(".", sep).TryToDouble();
                                double? l = record[4].Replace(".", sep).TryToDouble();
                                double? d = record[11].Replace(".", sep).TryToDouble();
                                double? a = record[12].Replace(".", sep).TryToDouble();

                                if (f.HasValue)
                                {
                                    refSig.Frequency_MHz = f.Value;

                                    if (l.HasValue)
                                        refSig.LevelSignal_dBm = l.Value;

                                    if (d.HasValue && a.HasValue)
                                    {
                                        refSig.IcsmTable = "MOB_STATION";
                                        if (!this.GetRefSignalBySensor(ref refSig, sensorLocation, d.Value, a.Value))
                                        {
                                            refSig.IcsmTable = "MOB_STATION2";
                                            this.GetRefSignalBySensor(ref refSig, sensorLocation, d.Value, a.Value);
                                        }
                                    }
                                }
                                listRefSig.Add(refSig);
                            }
                        }
                        var textBox = panel.Controls[$"path{sensorIndex}"];
                        textBox.Text = openFile.FileName;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Incorrect format file: " + openFile.FileName + "!\r\n" + "Line: " + i.ToString() + "\r\n" + e.Message);
                        _waitForm.Close();
                    }
                }

                refSit.ReferenceSignal = listRefSig.ToArray();
                refSit.SensorId = shortSensor.Id;
                _waitForm.Close();
            }
            return refSit;
        }
        private bool GetRefSignalBySensor(ref SDR.ReferenceSignal refSig, SDR.SensorLocation sensorLocation, double d, double a)
        {
            bool result = false;
            if (sensorLocation == null || !sensorLocation.Lon.HasValue || !sensorLocation.Lat.HasValue)
                return false;

            double lonSensor = sensorLocation.Lon.Value;
            double latSensor = sensorLocation.Lat.Value;

            double lon = lonSensor - d * Math.Sin(a * Math.PI / 180) / (111315 * Math.Cos(latSensor * Math.PI / 180));
            double lat = latSensor - d * Math.Cos(a * Math.PI / 180) / 111315;

            double mod = double.MaxValue;
            int eqpId = 0;

            string freqTableName = "";

            if (refSig.IcsmTable == "MOB_STATION")
                freqTableName = "MOBSTA_FREQS";
            else
                freqTableName = "MOBSTA_FREQS2";

            IMRecordset rs = new IMRecordset(freqTableName, IMRecordset.Mode.ReadOnly);
            rs.SetWhere("TX_FREQ", IMRecordset.Operation.Lt, refSig.Frequency_MHz + 0.0001);
            rs.SetWhere("TX_FREQ", IMRecordset.Operation.Gt, refSig.Frequency_MHz - 0.0001);
            rs.SetWhere("Station.Position.LONGITUDE", IMRecordset.Operation.Lt, lon + 0.1);
            rs.SetWhere("Station.Position.LONGITUDE", IMRecordset.Operation.Gt, lon - 0.1);
            rs.SetWhere("Station.Position.LATITUDE", IMRecordset.Operation.Lt, lat + 0.1);
            rs.SetWhere("Station.Position.LATITUDE", IMRecordset.Operation.Gt, lat - 0.1);
            rs.Select("ID,Station.BW, Station.Position.LONGITUDE,Station.Position.LATITUDE,Station.EQUIP_ID,Station.ID");
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                double staLon = rs.GetD("Station.Position.LONGITUDE");
                double staLat = rs.GetD("Station.Position.LATITUDE");

                if (Math.Abs(staLon - lon) + Math.Abs(staLat - lat) < mod)
                {
                    mod = Math.Abs(staLon - lon) + Math.Abs(staLat - lat);

                    double bw = rs.GetD("Station.BW");
                    int id = rs.GetI("ID");
                    eqpId = rs.GetI("Station.EQUIP_ID");

                    if (bw != 0 && bw != IM.NullD)
                        refSig.Bandwidth_kHz = bw;

                    refSig.IcsmId = rs.GetI("Station.ID");
                }
                result = true;
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            if (eqpId > 0)
            {
                var listFreq = new List<double>();
                var listLoss = new List<float>();
                string table;

                if (refSig.IcsmTable == "MOB_STATION")
                    table = "EQUIP_PMR_MPT";
                else
                    table = "EQUIP_MOB2_MPT";

                IMRecordset rsEqp = new IMRecordset(table, IMRecordset.Mode.ReadOnly);
                rsEqp.SetWhere("EQUIP_ID", IMRecordset.Operation.Eq, eqpId);
                rsEqp.SetWhere("TYPE", IMRecordset.Operation.Eq, "TS");
                rsEqp.Select("ATTN,FREQ");
                for (rsEqp.Open(); !rsEqp.IsEOF(); rsEqp.MoveNext())
                {
                    listLoss.Add((float)rsEqp.GetD("ATTN"));
                    listFreq.Add(1000 * rsEqp.GetD("FREQ"));
                }

                if (rsEqp.IsOpen())
                    rsEqp.Close();
                rsEqp.Destroy();

                if (listFreq.Count > 0 && listLoss.Count > 0)
                {
                    var signal = new SDR.SignalMask() { Freq_kHz = listFreq.ToArray(), Loss_dB = listLoss.ToArray() };
                    refSig.SignalMask = signal;
                }
            }
            return result;
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            IsPresOK = true;
            this.Close();
        }
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
