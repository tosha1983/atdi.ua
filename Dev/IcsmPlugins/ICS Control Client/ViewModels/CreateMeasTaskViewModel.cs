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

namespace XICSM.ICSControlClient.ViewModels
{
    public class CustomDataGrid : DataGrid
    {
        public CustomDataGrid()
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

        public static readonly DependencyProperty SelectedItemsListProperty = DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(CustomDataGrid), new PropertyMetadata(null));

        #endregion
    }
    public class CreateMeasTaskViewModel : WpfViewModelBase
    {
        #region Current Objects
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

        public CreateMeasTaskViewModel()
        {
            this.CreateMeasTaskCommand = new WpfCommand(this.OnCreateMeasTaskCommand);
            this._shortSensors = new ShortSensorDataAdatper();
            this._currentMeasTask = new MeasTaskViewModel();
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
            this._currentMeasTask.MeasDtParamTypeMeasurements = SDR.MeasurementType.Signaling;
            this._currentMeasTask.MeasTimeParamListPerStart = DateTime.Today;
            this._currentMeasTask.MeasTimeParamListPerStop = DateTime.Today.AddDays(1);
            this._currentMeasTask.MeasTimeParamListTimeStart = DateTime.Today;
            this._currentMeasTask.MeasTimeParamListTimeStop = DateTime.Today.AddDays(1).AddMinutes(-1);
            this._currentMeasTask.MeasTimeParamListPerInterval = 600;
            this._currentMeasTask.MeasFreqParamRgL = 900;
            this._currentMeasTask.MeasFreqParamRgU = 1000;
            this._currentMeasTask.MeasFreqParamStep = 100;
            this._currentMeasTask.MeasFreqParamMode = SDR.FrequencyMode.FrequencyRange;
            this._currentMeasTask.MeasDtParamRBW = 100;
            this._currentMeasTask.MeasDtParamVBW = 100;
            this._currentMeasTask.MeasDtParamMeasTime = 0.001;
            this._currentMeasTask.MeasDtParamDetectType = SDR.DetectingType.Avarage;
            this._currentMeasTask.MeasDtParamRfAttenuation = 0;
            this._currentMeasTask.MeasDtParamPreamplification = 0;
            this._currentMeasTask.MeasOtherTypeSpectrumOccupation = SDR.SpectrumOccupationType.FreqChannelOccupation;
            this._currentMeasTask.MeasOtherLevelMinOccup = -75;
            this._currentMeasTask.MeasOtherSwNumber = 10;
            this._currentMeasTask.MeasOtherTypeSpectrumScan = SDR.SpectrumScanType.Sweep;
            this._currentMeasTask.ResultType = SDR.MeasTaskResultType.MeasurementResult;
            this._currentMeasTask.ExecutionMode = SDR.MeasTaskExecutionMode.Automatic;
            this._currentMeasTask.Task = SDR.MeasTaskType.Scan;
            this._currentMeasTask.DateCreated = DateTime.Now;
            this._currentMeasTask.CreatedBy = IM.ConnectedUser();
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
                                double freqD;
                                if (double.TryParse(freq, out freqD))
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
                    foreach (ShortSensorViewModel shortSensor in this._currentShortSensor)
                    {
                        fm.OpenFileDialog openFile = new fm.OpenFileDialog() { Filter = "Текстовые файлы(*.csv)|*.csv", Title = shortSensor.Name };
                        if (openFile.ShowDialog() == fm.DialogResult.OK)
                        {
                            SDR.ReferenceSituation refSit = new SDR.ReferenceSituation();
                            List<SDR.ReferenceSignal> listRefSig = new List<SDR.ReferenceSignal>();

                            using (TextFieldParser parser = new TextFieldParser(openFile.FileName))
                            {
                                parser.TextFieldType = FieldType.Delimited;
                                parser.SetDelimiters(",");
                                int i = 0;
                                while (!parser.EndOfData)
                                {
                                    i++;
                                    var record = parser.ReadFields();

                                    if (i >= 4)
                                    {
                                        SDR.ReferenceSignal refSig = new SDR.ReferenceSignal();

                                        if (record[9].TryToDouble().HasValue)
                                        {
                                            refSig.Frequency_MHz = record[9].TryToDouble().Value;

                                            IMRecordset rs2 = new IMRecordset("MOBSTA_FREQS2", IMRecordset.Mode.ReadOnly);
                                            rs2.SetWhere("TX_FREQ", IMRecordset.Operation.Eq, record[9].TryToDouble().Value);
                                            rs2.Select("ID,Station.BW");
                                            for (rs2.Open(); !rs2.IsEOF(); rs2.MoveNext())
                                            {
                                                var bw = rs2.GetD("Station.BW");
                                                if (bw != 0 && bw != IM.NullD)
                                                    refSig.LevelSignal_dBm = bw;
                                            }

                                            if (refSig.LevelSignal_dBm == 0)
                                            {
                                                IMRecordset rs = new IMRecordset("MOBSTA_FREQS", IMRecordset.Mode.ReadOnly);
                                                rs.SetWhere("TX_FREQ", IMRecordset.Operation.Eq, record[9].TryToDouble().Value);
                                                rs.Select("ID,Station.BW");
                                                for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
                                                {
                                                    var bw = rs.GetD("Station.BW");
                                                    if (bw != 0 && bw != IM.NullD)
                                                        refSig.LevelSignal_dBm = bw;
                                                }
                                            }
                                        }


                                        if (record[4].TryToDouble().HasValue)
                                            refSig.LevelSignal_dBm = record[4].TryToDouble().Value;

                                        refSig.LevelSignal_dBm = 0;
                                        listRefSig.Add(refSig);
                                    }
                                }
                            }
                            refSit.ReferenceSignal = listRefSig.ToArray();
                            refSit.SensorId = shortSensor.Id;
                            listRef.Add(refSit);
                        }
                    }
                }


                List<SDR.MeasStation> stationsList = new List<SDR.MeasStation>();
                foreach (ShortSensorViewModel shortSensor in this._currentShortSensor)
                {
                    stationsList.Add(new SDR.MeasStation() { StationId = new SDR.MeasStationIdentifier() { Value = SVC.SdrnsControllerWcfClient.GetSensorById(shortSensor.Id).Id.Value } });
                };

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
                _measTaskForm.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private MP.MapDrawingDataPoint MakeDrawingPointForSensor(string status, double lon, double lat)
        {
            return new MP.MapDrawingDataPoint
            {
                Color = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
                Fill = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
                Location = new Models.Location(lon, lat),
                Opacity = 0.5,
                Height = 5,
                Width = 5
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
