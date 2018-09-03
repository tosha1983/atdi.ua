using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;
using XICSM.ICSControlClient.Models.WcfDataApadters;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using CS = XICSM.ICSControlClient.WpfControls.Charts;
using MP = XICSM.ICSControlClient.WpfControls.Maps;
using SDR = Atdi.AppServer.Contracts.Sdrns;
using System.Windows;
using FM = XICSM.ICSControlClient.Forms;
using ICSM;
using WCF = XICSM.ICSControlClient.WcfServiceClients;

namespace XICSM.ICSControlClient.ViewModels
{

    public class CreateMeasTaskViewModel : WpfViewModelBase
    {
        #region Current Objects

        // Tasks
        private MeasTaskViewModel _currentMeasTask;

        // Task -> Sensors
        private ShortSensorViewModel _currentShortSensor;

        // Task - Map (Task, TaskStation, TaskResult, TaskResultStation, TaskSensor)
        private MP.MapDrawingData _currentMapData;

        #endregion

        private ShortMeasTaskDataAdatper _shortMeasTasks;
        private ShortSensorDataAdatper _shortSensors;

        #region Commands

        public WpfCommand CreateMeasTaskCommand { get; set; }

        #endregion

        public CreateMeasTaskViewModel()
        {
            this.CreateMeasTaskCommand = new WpfCommand(this.OnCreateMeasTaskCommand);

            this._shortMeasTasks = new ShortMeasTaskDataAdatper();
            this._shortSensors = new ShortSensorDataAdatper();
            this._currentMeasTask = new MeasTaskViewModel();

            this.SetDefaultVaues();
            this.ReloadShortMeasTasks();
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

        public ShortSensorViewModel CurrentShortSensor
        {
            get => this._currentShortSensor;
            set => this.Set(ref this._currentShortSensor, value, RedrawMap);
        }

        #region Sources (Adapters)

        public ShortMeasTaskDataAdatper ShortMeasTasks => this._shortMeasTasks;

        public ShortSensorDataAdatper ShortSensors => this._shortSensors;

        #endregion

        private void SetDefaultVaues()
        {
            this._currentMeasTask.MeasDtParamTypeMeasurements = SDR.MeasurementType.Level;
            this._currentMeasTask.MeasTimeParamListPerStart = DateTime.Today;
            this._currentMeasTask.MeasTimeParamListPerStop = DateTime.Today.AddDays(1);
            this._currentMeasTask.MeasTimeParamListTimeStart = DateTime.Today;
            this._currentMeasTask.MeasTimeParamListTimeStop = DateTime.Today.AddDays(1).AddMinutes(-1);
            this._currentMeasTask.MeasTimeParamListPerInterval = 600;
            this._currentMeasTask.MeasFreqParamRgL = 900;
            this._currentMeasTask.MeasFreqParamRgU = 1000;
            this._currentMeasTask.MeasFreqParamStep = 100;
            this._currentMeasTask.MeasFreqParamMode = SDR.FrequencyMode.FrequencyRange;
            this._currentMeasTask.MeasDtParamRBW = 3;
            this._currentMeasTask.MeasDtParamVBW = 3;
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
        private void ReloadShortMeasTasks()
        {
            var sdrTasks = SVC.SdrnsControllerWcfClient.GetShortMeasTasks(new Atdi.AppServer.Contracts.DataConstraint());

            this._shortMeasTasks.Source = sdrTasks;
        }

        private void ReloadShortSensors()
        {
            var sdrSensors = SVC.SdrnsControllerWcfClient.GetShortSensors(new Atdi.AppServer.Contracts.DataConstraint());

            //var measTask = this.CurrentMeasTask;
            //if (measTask != null)
            //{
            //    if (measTask.Stations != null && measTask.Stations.Length > 0)
            //    {
            //        sdrSensors = sdrSensors
            //            .Where(sdrSensor => measTask.Stations
            //                    .FirstOrDefault(s => s.StationId.Value == sdrSensor.Id.Value) != null
            //                )
            //            .ToArray();
            //    }
            //    else
            //    {
            //        sdrSensors = new SDR.ShortSensor[] { };
            //    }
            //}
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
                    
                var measTask = new SDR.MeasTask()
                {
                    Name = this._currentMeasTask.Name,
                    MaxTimeBs = this._currentMeasTask.MaxTimeBs,
                    MeasFreqParam = new SDR.MeasFreqParam()
                    {
                        Mode = this.CurrentMeasTask.MeasFreqParamMode,
                        RgL = this.CurrentMeasTask.MeasFreqParamRgL,
                        RgU = this.CurrentMeasTask.MeasFreqParamRgU,
                        Step = this.CurrentMeasTask.MeasFreqParamStep
                        //MeasFreqs = new SDR.MeasFreq()
                    },
                    //MeasLocParams = ,
                    MeasOther = new SDR.MeasOther()
                    {
                        TypeSpectrumOccupation = this.CurrentMeasTask.MeasOtherTypeSpectrumOccupation,
                        LevelMinOccup = this.CurrentMeasTask.MeasOtherLevelMinOccup,
                        SwNumber = this.CurrentMeasTask.MeasOtherSwNumber,
                        TypeSpectrumScan = this.CurrentMeasTask.MeasOtherTypeSpectrumScan,
                        NChenal = this._currentMeasTask.MeasOtherNChenal
                    },
                    //MeasSubTasks = ,
                    OrderId = this._currentMeasTask.OrderId,
                    Prio = this._currentMeasTask.Prio,
                    ResultType = this.CurrentMeasTask.ResultType,
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

                    Stations = new SDR.MeasStation[] { new SDR.MeasStation()
                    {
                        StationId = new SDR.MeasStationIdentifier() { Value = SVC.SdrnsControllerWcfClient.GetSensorById(this._currentShortSensor.Id).Id.Value }
                    } } ,
                    //StationsForMeasurements = PreparedStationDataForMeasurements(tour, inspections),
                    Task = SDR.MeasTaskType.Scan,
                    ExecutionMode = SDR.MeasTaskExecutionMode.Automatic,
                    DateCreated = DateTime.Now,
                    CreatedBy = IM.ConnectedUser()
                };

                var measTaskId = WCF.SdrnsControllerWcfClient.CreateMeasTask(measTask);
                if (measTaskId == IM.NullI)
                {
                    throw new InvalidOperationException($"Could not create a meas task");
                }
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
                Location = new Models.Location(lon, lat)
            };
        }

        private void RedrawMap()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();

            if (this.CurrentShortSensor != null)
            {
                var svcSensor = SVC.SdrnsControllerWcfClient.GetSensorById(this.CurrentShortSensor.Id);
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
            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }
        
    }
}
