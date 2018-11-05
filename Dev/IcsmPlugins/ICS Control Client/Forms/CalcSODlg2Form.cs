using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using ICSM;
using System.Windows.Forms;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using FM = XICSM.ICSControlClient.Forms;
using SDR = Atdi.AppServer.Contracts.Sdrns;

namespace XICSM.ICSControlClient.Forms
{
    public partial class CalcSODlg2Form : Form
    {
        public SDR.ShortMeasurementResultsExtend[] _shortMeasResults;
        public int[] _planIds;
        public string[] _points;
        public int[] _allIds;
        public int _trigger;
        public CalcSODlg2Form()
        {
            InitializeComponent();
        }
        private void CalcSODlg2Form_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < _shortMeasResults.Count(); i++)
            {
                var ms = _shortMeasResults[i] as SDR.ShortMeasurementResultsExtend;
                dataGrid.Rows.Add(ms.Id.MeasSdrResultsId, ms.TimeMeas, ms.DataRank, ms.Number, ms.Status, ms.TypeMeasurements, ms.CurrentLon, ms.CurrentLat, ms.SensorName);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            List<int> measResultIds = new List<int>();

            for (int i = 0; i < dataGrid.SelectedRows.Count; i++)
            {
                var cc = dataGrid.SelectedRows[i].Cells[0].Value;
                int measResultId;

                if(cc != null && int.TryParse(cc.ToString(), out measResultId))
                {
                    measResultIds.Add(measResultId);
                }
            }

            SDR.SOFrequency[] soFrequency = new SDR.SOFrequency[] { };
            for (int i = 0; i < _planIds.Count(); i++ )
            {
                var planId = _planIds[i];
                var point = _points[i];
                var allot = _allIds[i];

                List<double> freqs = new List<double>();
                double bwz = IM.NullD;
                Dictionary<int, double> planFreqs = new Dictionary<int, double>();

                IMRecordset rsFq = new IMRecordset("CH_ALLOTED_CH", IMRecordset.Mode.ReadOnly);
                rsFq.SetWhere("ALLOT_ID", IMRecordset.Operation.Eq, allot);
                rsFq.Select("ID,FREQ");
                for (rsFq.Open(); !rsFq.IsEOF(); rsFq.MoveNext())
                {
                    var freq = rsFq.GetD("FREQ");
                    if (freq != 0 && freq != IM.NullD)
                        planFreqs.Add(rsFq.GetI("ID"), freq);
                }


                IMRecordset rs = new IMRecordset("FREQ_PLAN_CHAN", IMRecordset.Mode.ReadOnly);
                rs.SetWhere("PLAN_ID", IMRecordset.Operation.Eq, planId);
                rs.OrderBy("FREQ", OrderDirection.Ascending);
                rs.Select("PLAN_ID,FREQ,BANDWIDTH");
                for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
                {
                    if(bwz == IM.NullD)
                        bwz = rs.GetD("BANDWIDTH");

                    var freq = rs.GetD("FREQ");
                    if (freq != 0 && freq != IM.NullD)
                    {
                        if(planFreqs.Count() == 0 || planFreqs.ContainsValue(freq))
                            freqs.Add(freq);
                    }
                        
                }

                double LonMax = 0;
                double LonMin = 0;
                double LatMax = 0;
                double LatMin = 0;
                string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                if (!string.IsNullOrEmpty(point))
                {
                    foreach (var a in point.Split(new[] { "\r\n" }, StringSplitOptions.None))
                    {
                        if (!string.IsNullOrEmpty(a))
                        {
                            string[] b = a.Split(new[] { "\t" }, StringSplitOptions.None);
                            if (b.Length == 2)
                            {
                                double k1;
                                double k2;
                                if (double.TryParse(b[0].Replace(".", sep), out k1) && double.TryParse(b[1].Replace(".", sep), out k2))
                                {
                                    if (k1 > LonMax)
                                        LonMax = k1;
                                    if (k1 < LonMin || LonMin == 0)
                                        LonMin = k1;
                                    if (k2 > LatMax)
                                        LatMax = k2;
                                    if (k2 < LatMin || LatMin == 0)
                                        LatMin = k2;
                                }
                            }
                        }
                    }
                }
                else
                {
                    LatMax = 52;
                    LatMin = 44;
                    LonMax = 40;
                    LonMin = 22;
                }

                //using (StreamWriter outputFile = new StreamWriter(Path.Combine(@"C:\", "ICSMPluginLog.txt")))
                //{ 
                //    outputFile.WriteLine("<sdr:BW_kHz>:" + bwz.ToString() + "</sdr:BW_kHz>");
                //    outputFile.WriteLine("<sdr:LatMax>:" + LatMax.ToString() + "</sdr:LatMax>");
                //    outputFile.WriteLine("<sdr:LatMin>:" + LatMin.ToString() + "</sdr:LatMin>");
                //    outputFile.WriteLine("<sdr:LonMax>:" + LonMax.ToString() + "</sdr:LonMax>");
                //    outputFile.WriteLine("<sdr:LonMin>:" + LonMin.ToString() + "</sdr:LonMin>");
                //    outputFile.WriteLine("<sdr:TrLevel_dBm>:" + _trigger.ToString() + "</sdr:TrLevel_dBm>");
                //    outputFile.WriteLine("<sdr:MeasResultID>");
                //    foreach (var item in measResultIds)
                //    {
                //        outputFile.WriteLine("<arr:int>" + item.ToString() + "</arr:int>");
                //    }
                //    outputFile.WriteLine("</sdr:MeasResultID>");
                //    outputFile.WriteLine("<sdr:Frequencies_MHz>");
                //    foreach (var item in freqs)
                //    {
                //        outputFile.WriteLine("<arr:double>" + item.ToString() + "</arr:double>");
                //    }
                //    outputFile.WriteLine("</sdr:Frequencies_MHz>");
                //    outputFile.WriteLine("================================================");
                //}

                var currFrequency =  SVC.SdrnsControllerWcfClient.GetSOformMeasResultStation(freqs, bwz, measResultIds, LonMax, LonMin, LatMax, LatMin, _trigger);

                soFrequency = soFrequency.Concat(currFrequency).ToArray();
            }

            var dlgForm = new FM.CalcSODlg3Form();
            dlgForm._soFrequency = soFrequency;

            dlgForm.ShowDialog();
            dlgForm.Dispose();
            this.Close();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            dataGrid.SelectAll();
        }
    }
}
