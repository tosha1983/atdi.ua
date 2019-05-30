using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;
using XICSM.ICSControlClient.Models.WcfDataApadters;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using CS = XICSM.ICSControlClient.WpfControls.Charts;
using MP = XICSM.ICSControlClient.WpfControls.Maps;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Windows;
using FM = XICSM.ICSControlClient.Forms;
using ICSM;
using WCF = XICSM.ICSControlClient.WcfServiceClients;
using System.Windows.Controls;
using System.Collections;
using fm = System.Windows.Forms;
using Atdi.Common;
using System.Globalization;
using System.IO;

namespace XICSM.ICSControlClient.ViewModels
{
    public class CustomDataGridSensors : DataGrid
    {
        public CustomDataGridSensors()
        {
            this.SelectionChanged += CustomDataGrid_SelectionChanged;
        }

        void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedItemsList = this.SelectedItems;
        }
        #region SelectedItemsList

        public IList SelectedItemsList
        {
            get { return (IList)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty = DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(CustomDataGridSensors), new PropertyMetadata(null));

        #endregion
    }
    public class CreateMeasTaskViewModel : WpfViewModelBase
    {
        #region Current Objects
        private int? _allotId;
        private SDR.MeasurementType _measType = SDR.MeasurementType.Signaling;
        private MeasTaskViewModel _currentMeasTask;
        private IList _currentShortSensor;
        private MP.MapDrawingData _currentMapData;
        #endregion

        public FM.MeasTaskForm _measTaskForm;
        private ShortSensorDataAdatper _shortSensors;
        private double? _FreqParam;
        private string _FreqParams;

        #region Commands
        public WpfCommand CreateMeasTaskCommand { get; set; }
        #endregion

        public CreateMeasTaskViewModel(int? allotId)
        {
            this.CreateMeasTaskCommand = new WpfCommand(this.OnCreateMeasTaskCommand);
            this._shortSensors = new ShortSensorDataAdatper();
            this._currentMeasTask = new MeasTaskViewModel();
            this._allotId = allotId;
            this.SetDefaultVaues();
            this.ReloadShortSensors();
        }
        public MP.MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }
        public MeasTaskViewModel CurrentMeasTask
        {
            get => this._currentMeasTask;
            set => this.Set(ref this._currentMeasTask, value, () => { ReloadShortSensors(); RedrawMap(); });
        }
        public IList CurrentShortSensor
        {
            get => this._currentShortSensor;
            set
            {
                this._currentShortSensor = value;
                RedrawMap();
            }
        }
        public double? FreqParam
        {
            get => this._FreqParam;
            set => this.Set(ref this._FreqParam, value);
        }
        public string FreqParams
        {
            get => this._FreqParams;
            set => this.Set(ref this._FreqParams, value);
        }

        #region Sources (Adapters)
        public ShortSensorDataAdatper ShortSensors => this._shortSensors;
        #endregion
        private void SetDefaultVaues()
        {
            if (_measType == SDR.MeasurementType.Signaling && _allotId.HasValue)
            {
                int planId = IM.NullI;
                IMRecordset rsAllot = new IMRecordset("CH_ALLOTMENTS", IMRecordset.Mode.ReadOnly);
                rsAllot.SetWhere("ID", IMRecordset.Operation.Eq, _allotId.Value);
                rsAllot.Select("ID,CUST_TXT1,CUST_DAT1,CUST_DAT2,PLAN_ID,Plan.BANDWIDTH");
                for (rsAllot.Open(); !rsAllot.IsEOF(); rsAllot.MoveNext())
                {
                    this._currentMeasTask.Name = rsAllot.GetS("CUST_TXT1");
                    if (rsAllot.GetT("CUST_DAT1") != IM.NullT)
                        this._currentMeasTask.MeasTimeParamListPerStart = rsAllot.GetT("CUST_DAT1");
                    else
                        this._currentMeasTask.MeasTimeParamListPerStart = DateTime.Today;

                    if (rsAllot.GetT("CUST_DAT2") != IM.NullT)
                        this._currentMeasTask.MeasTimeParamListPerStop = rsAllot.GetT("CUST_DAT2");
                    else
                        this._currentMeasTask.MeasTimeParamListPerStop = DateTime.Today.AddDays(1);

                    if (rsAllot.GetD("Plan.BANDWIDTH") != IM.NullD)
                        this._currentMeasTask.MeasFreqParamStep = rsAllot.GetD("Plan.BANDWIDTH");
                    else
                        this._currentMeasTask.MeasFreqParamStep = 100;
                    planId = rsAllot.GetI("PLAN_ID");
                }

                if (planId != IM.NullI)
                {
                    double? minFq = null;
                    double? maxFq = null;

                    IMRecordset rs = new IMRecordset("FREQ_PLAN_CHAN", IMRecordset.Mode.ReadOnly);
                    rs.SetWhere("PLAN_ID", IMRecordset.Operation.Eq, planId);
                    rs.Select("PLAN_ID,FREQ");
                    for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
                    {
                        if (rs.GetD("FREQ") != IM.NullD)
                        {
                            if (!minFq.HasValue || minFq.Value > rs.GetD("FREQ"))
                                minFq = rs.GetD("FREQ");
                            if (!maxFq.HasValue || maxFq.Value < rs.GetD("FREQ"))
                                maxFq = rs.GetD("FREQ");
                        }
                    }
                    if (minFq.HasValue)
                        this._currentMeasTask.MeasFreqParamRgL = minFq.Value;
                    else
                        this._currentMeasTask.MeasFreqParamRgL = 900;

                    if (maxFq.HasValue)
                        this._currentMeasTask.MeasFreqParamRgU = maxFq.Value;
                    else
                        this._currentMeasTask.MeasFreqParamRgU = 1000;
                }
                else
                {
                    this._currentMeasTask.MeasFreqParamRgL = 900;
                    this._currentMeasTask.MeasFreqParamRgU = 1000;
                    this._currentMeasTask.MeasFreqParamStep = 100;
                }
                this._currentMeasTask.MeasFreqParamMode = SDR.FrequencyMode.FrequencyRange;
            }
            else
            {
                this._currentMeasTask.MeasTimeParamListPerStart = DateTime.Today;
                this._currentMeasTask.MeasTimeParamListPerStop = DateTime.Today.AddDays(1);
                this._currentMeasTask.MeasFreqParamMode = SDR.FrequencyMode.FrequencyRange;
                this._currentMeasTask.MeasFreqParamRgL = 900;
                this._currentMeasTask.MeasFreqParamRgU = 1000;
                this._currentMeasTask.MeasFreqParamStep = 100;
            }

            this._currentMeasTask.MeasDtParamTypeMeasurements = SDR.MeasurementType.Signaling;
            this._currentMeasTask.MeasTimeParamListTimeStart = DateTime.Today;
            this._currentMeasTask.MeasTimeParamListTimeStop = DateTime.Today.AddDays(1).AddMinutes(-1);
            this._currentMeasTask.MeasTimeParamListPerInterval = 600;
            if (_measType == SDR.MeasurementType.Signaling)
            {
                this._currentMeasTask.MeasDtParamRBW = null;
                this._currentMeasTask.MeasDtParamVBW = null;
                this._currentMeasTask.MeasDtParamMeasTime = null;
                this._currentMeasTask.MeasDtParamMeasTime = 0.001;
                this._currentMeasTask.MeasDtParamDetectType = SDR.DetectingType.Peak;
                //this._currentMeasTask.MeasDtParamRfAttenuation = 0;
                //this._currentMeasTask.MeasDtParamPreamplification = 0;
            }
            else
            {
                this._currentMeasTask.MeasDtParamRBW = 100;
                this._currentMeasTask.MeasDtParamVBW = 100;
                this._currentMeasTask.MeasDtParamMeasTime = 0.001;
                this._currentMeasTask.MeasDtParamDetectType = SDR.DetectingType.Avarage;
                this._currentMeasTask.MeasDtParamRfAttenuation = 0;
                this._currentMeasTask.MeasDtParamPreamplification = 0;
            }
            this._currentMeasTask.MeasOtherTypeSpectrumOccupation = SDR.SpectrumOccupationType.FreqChannelOccupation;
            this._currentMeasTask.MeasOtherLevelMinOccup = -75;
            this._currentMeasTask.MeasOtherSwNumber = 10;
            this._currentMeasTask.MeasOtherTypeSpectrumScan = SDR.SpectrumScanType.Sweep;
            this._currentMeasTask.ResultType = SDR.MeasTaskResultType.MeasurementResult;
            this._currentMeasTask.ExecutionMode = SDR.MeasTaskExecutionMode.Automatic;
            this._currentMeasTask.Task = SDR.MeasTaskType.Scan;
            this._currentMeasTask.DateCreated = DateTime.Now;
            this._currentMeasTask.CreatedBy = IM.ConnectedUser();
            this._currentMeasTask.CompareTraceJustWithRefLevels = false;
            this._currentMeasTask.AutoDivisionEmitting = true;
            this._currentMeasTask.DifferenceMaxMax = 20;
            this._currentMeasTask.FiltrationTrace = true;
            this._currentMeasTask.AllowableExcess_dB = 10;
            this._currentMeasTask.SignalizationNChenal = 50;
            this._currentMeasTask.SignalizationNCount = 1000000;
        }
        private void ReloadShortSensors()
        {
            var sdrSensors = SVC.SdrnsControllerWcfClient.GetShortSensors();
            this._shortSensors.Source = sdrSensors;
        }
        private void OnCreateMeasTaskCommand(object parameter)
        {
            try
            {
                if (this._currentShortSensor == null)
                {
                    MessageBox.Show("Undefined sensor!");
                    return;
                }

                if (this._currentMeasTask.MeasTimeParamListPerStart > this._currentMeasTask.MeasTimeParamListPerStop)
                {
                    MessageBox.Show("Date Stop should be great of the Date Start!");
                    return;
                }
                if (this._currentMeasTask.MeasTimeParamListTimeStart > this._currentMeasTask.MeasTimeParamListTimeStop)
                {
                    MessageBox.Show("Time Stop should be great of the Time Start!");
                    return;
                }
                //if (this._currentMeasTask.MeasTimeParamListPerInterval <= 0 || this._currentMeasTask.MeasTimeParamListPerInterval >= 3600)
                //{
                //    MessageBox.Show("Incorrect value Duration!");
                //    return;
                //}
                if (this._currentMeasTask.MeasFreqParamRgL > this._currentMeasTask.MeasFreqParamRgU)
                {
                    MessageBox.Show("Stop freq should be great of the Start freq!");
                    return;
                }
                if (this._currentMeasTask.MeasFreqParamRgL <= 1 || this._currentMeasTask.MeasFreqParamRgL >= 6000)
                {
                    MessageBox.Show("Incorrect value Start freq!");
                    return;
                }
                if (this._currentMeasTask.MeasFreqParamRgU <= 1 || this._currentMeasTask.MeasFreqParamRgU >= 6000)
                {
                    MessageBox.Show("Incorrect value Stop freq!");
                    return;
                }
                if (this._currentMeasTask.MeasFreqParamStep <= 0 || this._currentMeasTask.MeasFreqParamStep >= 20000)
                {
                    MessageBox.Show("Incorrect value Step whith!");
                    return;
                }
                if (this._currentMeasTask.MeasDtParamRBW <= 0.001 || this._currentMeasTask.MeasDtParamRBW >= 10000)
                {
                    MessageBox.Show("Incorrect value RBW!");
                    return;
                }
                if (this._currentMeasTask.MeasDtParamVBW <= 0.001 || this._currentMeasTask.MeasDtParamVBW >= 10000)
                {
                    MessageBox.Show("Incorrect value VBW!");
                    return;
                }
                if (this._currentMeasTask.MeasDtParamVBW > this._currentMeasTask.MeasDtParamRBW)
                {
                    MessageBox.Show("VBW should be great of the RBW!");
                    return;
                }
                if (this._currentMeasTask.MeasDtParamMeasTime < 0.001 || this._currentMeasTask.MeasDtParamMeasTime > 1)
                {
                    MessageBox.Show("Incorrect value Sweep time!");
                    return;
                }
                if (this._currentMeasTask.MeasDtParamRfAttenuation != 0 && this._currentMeasTask.MeasDtParamRfAttenuation != 10 && this._currentMeasTask.MeasDtParamRfAttenuation != 20 && this._currentMeasTask.MeasDtParamRfAttenuation != 30)
                {
                    MessageBox.Show("Incorrect value Attenuation!");
                    return;
                }
                if (this._currentMeasTask.MeasDtParamPreamplification != 0 && this._currentMeasTask.MeasDtParamPreamplification != 10 && this._currentMeasTask.MeasDtParamPreamplification != 20 && this._currentMeasTask.MeasDtParamPreamplification != 30)
                {
                    MessageBox.Show("Incorrect value Gain of preamplifier!");
                    return;
                }
                if (this._currentMeasTask.MeasOtherLevelMinOccup <= -130 || this._currentMeasTask.MeasOtherLevelMinOccup >= -30)
                {
                    MessageBox.Show("Incorrect value Level occupation!");
                    return;
                }
                if (this._currentMeasTask.MeasOtherSwNumber < 1 || this._currentMeasTask.MeasOtherSwNumber > 10000)
                {
                    MessageBox.Show("Incorrect value Number of scan!");
                    return;
                }

                var measFreqParam = new SDR.MeasFreqParam() { Mode = this._currentMeasTask.MeasFreqParamMode, Step = this._currentMeasTask.MeasFreqParamStep };

                switch (measFreqParam.Mode)
                {
                    case SDR.FrequencyMode.SingleFrequency:
                        if (FreqParam.HasValue)
                        {
                            measFreqParam.MeasFreqs = new SDR.MeasFreq[] { new SDR.MeasFreq() { Freq = FreqParam.Value } };
                        }
                        break;
                    case SDR.FrequencyMode.FrequencyList:
                        if (!string.IsNullOrEmpty(FreqParams))
                        {
                            var freqArray = FreqParams.Replace(';', ',').Split(',');
                            foreach (var freq in freqArray)
                            {
                                if (double.TryParse(freq, out double freqD))
                                {
                                    measFreqParam.MeasFreqs = measFreqParam.MeasFreqs.Concat(new SDR.MeasFreq[] { new SDR.MeasFreq() { Freq = freqD } }).ToArray();
                                }
                            }
                        }
                        break;
                    case SDR.FrequencyMode.FrequencyRange:
                        measFreqParam.RgL = this._currentMeasTask.MeasFreqParamRgL;
                        measFreqParam.RgU = this._currentMeasTask.MeasFreqParamRgU;
                        //measFreqParam.Step = this._currentMeasTask.MeasFreqParamStep;
                        break;
                }

                List<SDR.ReferenceSituation> listRef = new List<SDR.ReferenceSituation>();

                if (this._currentMeasTask.MeasDtParamTypeMeasurements == SDR.MeasurementType.Signaling)
                {
                    string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                    foreach (ShortSensorViewModel shortSensor in this._currentShortSensor)
                    {
                        fm.OpenFileDialog openFile = new fm.OpenFileDialog() { Filter = "Текстовые файлы(*.csv)|*.csv", Title = shortSensor.Name };
                        if (openFile.ShowDialog() == fm.DialogResult.OK)
                        {
                            var _waitForm = new FM.WaitForm();
                            _waitForm.SetMessage("Loading file. Please wait...");
                            _waitForm.TopMost = true;
                            _waitForm.Show();
                            _waitForm.Refresh();

                            SDR.ReferenceSituation refSit = new SDR.ReferenceSituation();
                            List<SDR.ReferenceSignal> listRefSig = new List<SDR.ReferenceSignal>();

                            var svcSensor = SVC.SdrnsControllerWcfClient.GetSensorById(shortSensor.Id);
                            SDR.SensorLocation sensorLocation = null;
                            if (svcSensor.Locations != null)
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
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show("Incorrect format file: " + openFile.FileName + "!\r\n" + "Line: " + i.ToString() + "\r\n" + e.Message);
                                    _waitForm.Close();
                                    return;
                                }
                            }

                            refSit.ReferenceSignal = listRefSig.ToArray();
                            refSit.SensorId = shortSensor.Id;
                            listRef.Add(refSit);
                            _waitForm.Close();
                        }
                    }
                }


                List<SDR.MeasStation> stationsList = new List<SDR.MeasStation>();
                foreach (ShortSensorViewModel shortSensor in this._currentShortSensor)
                {
                    stationsList.Add(new SDR.MeasStation() { StationId = new SDR.MeasStationIdentifier() { Value = SVC.SdrnsControllerWcfClient.GetSensorById(shortSensor.Id).Id.Value } });
                }

                var measTask = new SDR.MeasTask()
                {
                    Name = this._currentMeasTask.Name,
                    MaxTimeBs = this._currentMeasTask.MaxTimeBs,
                    MeasFreqParam = measFreqParam,
                    //MeasFreqParam = new SDR.MeasFreqParam()
                    //{
                    //    Mode = this._currentMeasTask.MeasFreqParamMode,
                    //    RgL = this._currentMeasTask.MeasFreqParamRgL,
                    //    RgU = this._currentMeasTask.MeasFreqParamRgU,
                    //    Step = this._currentMeasTask.MeasFreqParamStep
                    //    //MeasFreqs = new SDR.MeasFreq()
                    //},
                    //MeasLocParams = ,
                    MeasOther = new SDR.MeasOther()
                    {
                        TypeSpectrumOccupation = this._currentMeasTask.MeasOtherTypeSpectrumOccupation,
                        LevelMinOccup = this._currentMeasTask.MeasOtherLevelMinOccup,
                        SwNumber = this._currentMeasTask.MeasOtherSwNumber,
                        TypeSpectrumScan = this._currentMeasTask.MeasOtherTypeSpectrumScan,
                        NChenal = this._currentMeasTask.MeasOtherNChenal
                    },
                    //MeasSubTasks = ,
                    OrderId = this._currentMeasTask.OrderId,
                    Prio = this._currentMeasTask.Prio,
                    ResultType = this._currentMeasTask.ResultType,
                    Status = this._currentMeasTask.Status,
                    Type = this._currentMeasTask.Type,
                    MeasDtParam = new SDR.MeasDtParam()
                    {
                        TypeMeasurements = this._currentMeasTask.MeasDtParamTypeMeasurements,
                        RBW = this._currentMeasTask.MeasDtParamRBW,
                        VBW = this._currentMeasTask.MeasDtParamVBW,
                        MeasTime = this._currentMeasTask.MeasDtParamMeasTime,
                        DetectType = this._currentMeasTask.MeasDtParamDetectType,
                        RfAttenuation = this._currentMeasTask.MeasDtParamRfAttenuation,
                        Preamplification = this._currentMeasTask.MeasDtParamPreamplification,
                        Mode = this._currentMeasTask.MeasDtParamMode,
                        Demod = this._currentMeasTask.MeasDtParamDemod,
                        IfAttenuation = this._currentMeasTask.MeasDtParamIfAttenuation
                    },
                    MeasTimeParamList = new SDR.MeasTimeParamList()
                    {
                        PerStart = this._currentMeasTask.MeasTimeParamListPerStart,
                        PerStop = this._currentMeasTask.MeasTimeParamListPerStop,
                        TimeStart = this._currentMeasTask.MeasTimeParamListTimeStart,
                        TimeStop = this._currentMeasTask.MeasTimeParamListTimeStop,
                        Days = this._currentMeasTask.MeasTimeParamListDays,
                        PerInterval = this._currentMeasTask.MeasTimeParamListPerInterval
                    },
                    SignalingMeasTaskParameters = new SDR.SignalingMeasTask()
                    {
                        allowableExcess_dB = this._currentMeasTask.AllowableExcess_dB,
                        AutoDivisionEmitting = this._currentMeasTask.AutoDivisionEmitting,
                        CompareTraceJustWithRefLevels = this._currentMeasTask.CompareTraceJustWithRefLevels,
                        DifferenceMaxMax = this._currentMeasTask.DifferenceMaxMax,
                        FiltrationTrace = this._currentMeasTask.FiltrationTrace,
                        SignalizationNChenal = this._currentMeasTask.SignalizationNChenal,
                        SignalizationNCount = this._currentMeasTask.SignalizationNCount
                    },
                    Stations = stationsList.ToArray(),
                    Task = SDR.MeasTaskType.Scan,
                    ExecutionMode = SDR.MeasTaskExecutionMode.Automatic,
                    DateCreated = DateTime.Now,
                    CreatedBy = IM.ConnectedUser(),
                    RefSituation = listRef.ToArray()
                };

                var measTaskId = WCF.SdrnsControllerWcfClient.CreateMeasTask(measTask);
                if (measTaskId == IM.NullI)
                {
                    throw new InvalidOperationException($"Could not create a meas task");
                }
                else
                {
                    IMRecordset rsAllot = new IMRecordset("CH_ALLOTMENTS", IMRecordset.Mode.ReadWrite);
                    rsAllot.Select("ID,STATUS,CUST_NBR1");
                    rsAllot.SetWhere("ID", IMRecordset.Operation.Eq, _allotId.Value);
                    using (rsAllot.OpenWithScope())
                    {
                        rsAllot.Edit();
                        rsAllot.Put("STATUS", "dur");
                        rsAllot.Put("CUST_NBR1", measTaskId);
                        rsAllot.Update();
                    }
                }
                _measTaskForm.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
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
        private MP.MapDrawingDataPoint MakeDrawingPointForSensor(string status, double lon, double lat)
        {
            return new MP.MapDrawingDataPoint
            {
                Color = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
                Fill = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
                Location = new Models.Location(lon, lat),
                Opacity = 0.85,
                Width = 10,
                Height = 10
            };
        }
        private void RedrawMap()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();

            if (this._currentShortSensor != null)
            {
                foreach (ShortSensorViewModel shortSensor in this._currentShortSensor)
                {
                    var svcSensor = SVC.SdrnsControllerWcfClient.GetSensorById(shortSensor.Id);
                    if (svcSensor != null)
                    {
                        var modelSensor = Mappers.Map(svcSensor);
                        if (modelSensor.Locations != null && modelSensor.Locations.Length > 0)
                        {
                            var sensorPoints = modelSensor.Locations
                                .Where(l => ("A".Equals(l.Status, StringComparison.OrdinalIgnoreCase)
                                        || "Z".Equals(l.Status, StringComparison.OrdinalIgnoreCase))
                                        && l.Lon.HasValue
                                        && l.Lat.HasValue)
                                .Select(l => this.MakeDrawingPointForSensor(l.Status, l.Lon.Value, l.Lat.Value))
                                .ToArray();

                            points.AddRange(sensorPoints);
                        }
                    }
                }
            }
            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }
    }
}
