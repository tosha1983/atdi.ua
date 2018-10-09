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

namespace XICSM.ICSControlClient.ViewModels
{

    public class MeasResultsViewModel : WpfViewModelBase
    {
        public enum ModelType
        {
            None,
            Tasks,
            TaskStations,
            TaskResults,
            TaskResultStations,
            Sensors
        }



        #region Corrent Objects

        // Tasks
        private MeasTaskViewModel _currentMeasTask;
        private ShortMeasTaskViewModel _currentShortMeasTask;

        // Task -> TaskStation
        private MeasTaskDetailStationViewModel _currentMeasTaskStation;

        // Task -> Sensors
        private ShortSensorViewModel _currentShortSensor;

        // Task -> Results
        private MeasurementResultsViewModel _currentMeasurementResults;
        private ShortMeasurementResultsViewModel _currentShortMeasurementResults;

        // Task -> Results - Stations
        private ResultsMeasurementsStationViewModel _currentResultsMeasurementsStation;

        // Task -> Results -> chart
        private CS.ChartOption _currentChartOption;

        // Task - Map (Task, TaskStation, TaskResult, TaskResultStation, TaskSensor)
        private MP.MapDrawingData _currentMapData;

        // The current model
        private ModelType _currentModel;

        #endregion

        private ShortMeasTaskDataAdatper _shortMeasTasks;
        private ShortMeasurementResultsDataAdatper _shortMeasResults;
        private ShortSensorDataAdatper _shortSensors;
        private MeasTaskDetailStationDataAdapter _measTaskDetailStations;
        private ResultsMeasurementsStationDataAdapter _resultsMeasurementsStations;
        private LevelMeasurementsCarDataAdapter _levelMeasurements;

        private Visibility _measTaskDetailVisibility = Visibility.Hidden;
        private Visibility _measResultsDetailVisibility = Visibility.Hidden;

        #region Commands

        public WpfCommand CreateMeasTaskCommand { get; set; }
        public WpfCommand DeleteMeasTaskCommand { get; set; }
        public WpfCommand RunMeasTaskCommand { get; set; }
        public WpfCommand StopMeasTaskCommand { get; set; }

        public WpfCommand RefreshShortTasksCommand { get; set; }

        public WpfCommand RefreshShortSensorsCommand { get; set; }

        public WpfCommand ShowHideMeasTaskDetailCommand { get; set; }

        public WpfCommand ShowHideMeasResultsDetailCommand { get; set; }
        public WpfCommand MeasResultCommand { get; set; }

        #endregion

        private CS.ChartOption GetDefaultChartOption()
        {
            return new CS.ChartOption
            {
                ChartType = CS.ChartType.Line,
                XLabel = "Freq (Mhz)",
                XMin = 900,
                XMax = 960,
                XTick = 5,
                XInnerTickCount = 5,
                YLabel = "Level (dBm)",
                YMin = -120,
                YMax = -10,
                YTick = 10,
                YInnerTickCount = 5,
                Title = "Measurements"
            };
        }

        public MeasResultsViewModel()
        {
            this._currentModel = ModelType.None;
            this._currentChartOption = this.GetDefaultChartOption();
            this._shortMeasTasks = new ShortMeasTaskDataAdatper();
            this._shortMeasResults = new ShortMeasurementResultsDataAdatper();
            this._shortSensors = new ShortSensorDataAdatper();
            this._measTaskDetailStations = new MeasTaskDetailStationDataAdapter();
            this._resultsMeasurementsStations = new ResultsMeasurementsStationDataAdapter();
            this._levelMeasurements = new LevelMeasurementsCarDataAdapter();

            this.MeasDtParamTypeMeasurements = SDR.MeasurementType.MonitoringStations;
        }
        public SDR.MeasurementType MeasDtParamTypeMeasurements { get; set; }
        public IList<SDR.MeasurementType> MeasDtParamTypeMeasurementsValues
        {
            get { return Enum.GetValues(typeof(SDR.MeasurementType)).Cast<SDR.MeasurementType>().ToList<SDR.MeasurementType>(); }
        }
        public ModelType CurrentModel
        {
            get => this._currentModel;
            set => this.Set(ref this._currentModel, value);
        }


        public CS.ChartOption CurrentChartOption
        {
            get => this._currentChartOption;
            set => this.Set(ref this._currentChartOption, value);
        }

        public MP.MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }


        public ShortSensorViewModel CurrentShortSensor
        {
            get => this._currentShortSensor;
            set => this.Set(ref this._currentShortSensor, value, RedrawMap);
        }

        public MeasTaskDetailStationViewModel CurrentMeasTaskStation
        {
            get => this._currentMeasTaskStation;
            set => this.Set(ref this._currentMeasTaskStation, value, RedrawMap);
        }

        public MeasurementResultsViewModel CurrentMeasurementResults
        {
            get => this._currentMeasurementResults;
            set => this.Set(ref this._currentMeasurementResults, value, () => { UpdateCurrentChartOption(); this.RedrawMap(); });
        }

        public ShortMeasurementResultsViewModel CurrentShortMeasurementResults
        {
            get => this._currentShortMeasurementResults;
            set => this.Set(ref this._currentShortMeasurementResults, value, ReloadMeasResaltDetail);
        }

        public ResultsMeasurementsStationViewModel CurrentResultsMeasurementsStation
        {
            get => this._currentResultsMeasurementsStation;
            set => this.Set(ref this._currentResultsMeasurementsStation, value, () => { ReloadLevelMeasurements(); UpdateCurrentChartOption(); });
        }

        #region Sources (Adapters)

        public LevelMeasurementsCarDataAdapter LevelMeasurements => this._levelMeasurements;

        public ShortMeasTaskDataAdatper ShortMeasTasks => this._shortMeasTasks;

        public ShortMeasurementResultsDataAdatper ShortMeasResults => this._shortMeasResults;

        //public ShortMeasResultsSpecial => this._ShortMeasResultsSpecial GetShortMeasResultsSpecial

        public ShortSensorDataAdatper ShortSensors => this._shortSensors;

        public MeasTaskDetailStationDataAdapter MeasTaskDetailStations => this._measTaskDetailStations;

        public ResultsMeasurementsStationDataAdapter ResultsMeasurementsStations => this._resultsMeasurementsStations;

        #endregion


        private void ReloadShortMeasTasks()
        {
            var sdrTasks = SVC.SdrnsControllerWcfClient.GetShortMeasTasks(new Atdi.AppServer.Contracts.DataConstraint());

            this._shortMeasTasks.Source = sdrTasks;
        }

        private void ReloadShortMeasResults()
        {
            int taskId = 0;
            if (this._currentShortMeasTask != null)
            {
                taskId = this._currentShortMeasTask.Id;
            }

            var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetShortMeasResultsByTask(taskId);

            this._shortMeasResults.Source = sdrMeasResults;
            this._resultsMeasurementsStations.Source = null;
        }

        private void ReloadMeasResaltDetail()
        {
            //this.MeasDtParamTypeMeasurements = SDR.MeasurementType.MonitoringStations;
            if (this._currentShortMeasurementResults != null)
            {
                var measResults = SVC.SdrnsControllerWcfClient
                        .GetMeasResultsById(
                            this._currentShortMeasurementResults.MeasSdrResultsId,
                            this._currentShortMeasurementResults.MeasTaskId,
                            this._currentShortMeasurementResults.SubMeasTaskId,
                            this._currentShortMeasurementResults.SubMeasTaskStationId);

                var measResultsViewModel = Mappers.Map(measResults);
                this.CurrentMeasurementResults = measResultsViewModel;

                if (measResultsViewModel != null)
                {
                    this._resultsMeasurementsStations.Source = measResultsViewModel.ResultsMeasStation;
                }
                else
                {
                    this._resultsMeasurementsStations.Source = null;
                }
            }
            else
            {
                this.CurrentMeasurementResults = null;
                this._resultsMeasurementsStations.Source = null;
            }


        }

        private void ReloadLevelMeasurements()
        {
            if (this.CurrentResultsMeasurementsStation == null)
            {
                this._levelMeasurements.Source = null;
                return;
            }

            this._levelMeasurements.Source = this.CurrentResultsMeasurementsStation.LevelMeasurements;
        }

        private void UpdateCurrentChartOption()
        {
            if (this.CurrentMeasurementResults == null)
            {
                this.CurrentChartOption = this.GetDefaultChartOption();
            }
            else
            {
                this.CurrentChartOption = this.GetChartOption(this.CurrentMeasurementResults);
            }
        }

        private CS.ChartOption GetChartOption(MeasurementResultsViewModel result)
        {
            if (result.TypeMeasurements == SDR.MeasurementType.MonitoringStations)
            {
                return this.GetChartOptionByMonitoringStations(result);
            }
            else if (result.TypeMeasurements == SDR.MeasurementType.SpectrumOccupation)
            {
                return this.GetChartOptionBySpectrumOccupation(result);
            }
            else if (result.TypeMeasurements == SDR.MeasurementType.Level)
            {
                return this.GetChartOptionByLevel(result);
            }
            return this.GetDefaultChartOption();

        }

        private CS.ChartOption GetChartOptionByMonitoringStations(MeasurementResultsViewModel result)
        {
            var option = new CS.ChartOption
            {
                Title = "Monitoring Stations",
                YLabel = "Level (dBm)",
                XLabel = "Freq (Mhz)",
                ChartType = CS.ChartType.Line,
                XInnerTickCount = 5,
                YInnerTickCount = 5,
                YMin = -120,
                YMax = -10,
                XMin = 900,
                XMax = 960,
                YTick = 10,
                XTick = 10
            };

            var measStation = this.CurrentResultsMeasurementsStation;
            if (measStation == null)
            {
                return option;
            }

            var spectrumLevels = measStation.GeneralResultLevelsSpecrum;
            if (spectrumLevels == null || spectrumLevels.Length == 0)
            {
                return option;
            }

            var count = spectrumLevels.Length;
            var points = new Point[count];
            var maxX = default(double);
            var minX = default(double);

            var maxY = default(double);
            var minY = default(double);
            for (int i = 0; i < count; i++)
            {
                var valX = Convert.ToDouble(measStation.GeneralResultSpecrumStartFreq + i * measStation.GeneralResultSpecrumSteps / 1000);
                var valY = spectrumLevels[i];

                var point = new Point
                {
                    X = valX,
                    Y = valY
                };
                if (i == 0)
                {
                    maxX = valX;
                    minX = valX;
                    maxY = valY;
                    minY = valY;
                }
                else
                {
                    if (maxX < valX)
                        maxX = valX;
                    if (minX > valX)
                        minX = valX;

                    if (maxY < valY)
                        maxY = valY;
                    if (minY > valY)
                        minY = valY;
                }
                points[i] = point;
            }

            var preparedDataY = Environment.Utitlity.CalcLevelRange(minY, maxY);
            option.YTick = 10;
            option.YMax = preparedDataY.MaxValue;
            option.YMin = preparedDataY.MinValue;

            var preparedDataX = Environment.Utitlity.CalcFrequencyRange(minX, maxX, 8);
            option.XTick = preparedDataX.Step;
            option.XMin = preparedDataX.MinValue;
            option.XMax = preparedDataX.MaxValue;

            option.Points = points;

            return option;
        }

        private CS.ChartOption GetChartOptionBySpectrumOccupation(MeasurementResultsViewModel result)
        {
            var option = new CS.ChartOption
            {
                Title = "Spectrum Occupation",
                YLabel = "Occupation (%)",
                XLabel = "Freq (Mhz)",
                ChartType = CS.ChartType.Columns,
                XInnerTickCount = 5,
                YInnerTickCount = 5,
                YMin = 0,
                YMax = 100,
                XMin = 900,
                XMax = 960,
                YTick = 10,
                XTick = 10
            };

            var count = result.FrequenciesMeasurements.Length;
            var points = new Point[count];
            var max = default(double);
            var min = default(double);
            for (int i = 0; i < count; i++)
            {
                var ms = result.MeasurementsResults[i] as SDR.SpectrumOccupationMeasurementResult;
                var valX = result.FrequenciesMeasurements[i].Freq;
                var valY = ms.Occupancy ?? 0;
                var point = new Point
                {
                    X = valX,
                    Y = valY
                };
                if (i == 0)
                {
                    max = valX;
                    min = valX;
                }
                else
                {
                    if (max < valX)
                        max = valX;
                    if (min > valX)
                        min = valX;
                }
                points[i] = point;
            }

            var preparedDataX = Environment.Utitlity.CalcFrequencyRange(min, max, 6);
            option.XTick = preparedDataX.Step;
            option.XMin = preparedDataX.MinValue;
            option.XMax = preparedDataX.MaxValue;

            option.Points = points;
            return option;
        }

        private CS.ChartOption GetChartOptionByLevel(MeasurementResultsViewModel result)
        {
            var option = new CS.ChartOption
            {
                Title = "Level",
                YLabel = "Level (dBm)",
                XLabel = "Freq (Mhz)",
                ChartType = CS.ChartType.Line,
                XInnerTickCount = 5,
                YInnerTickCount = 5,
                YMin = -120,
                YMax = -10,
                XMin = 900,
                XMax = 960,
                YTick = 10,
                XTick = 10
            };

            var count = result.FrequenciesMeasurements.Length;
            var points = new Point[count];

            var maxX = default(double);
            var minX = default(double);

            var maxY = default(double);
            var minY = default(double);

            for (int i = 0; i < count; i++)
            {
                var ms = result.MeasurementsResults[i] as SDR.LevelMeasurementResult;
                var valX = result.FrequenciesMeasurements[i].Freq;
                var valY = ms.Value ?? 0;
                var point = new Point
                {
                    X = valX,
                    Y = valY
                };
                if (i == 0)
                {
                    maxX = valX;
                    minX = valX;
                    maxY = valY;
                    minY = valY;
                }
                else
                {
                    if (maxX < valX)
                        maxX = valX;
                    if (minX > valX)
                        minX = valX;

                    if (maxY < valY)
                        maxY = valY;
                    if (minY > valY)
                        minY = valY;
                }
                points[i] = point;
            }

            var preparedDataY = Environment.Utitlity.CalcLevelRange(minY, maxY);
            option.YTick = 10;
            option.YMax = preparedDataY.MaxValue;
            option.YMin = preparedDataY.MinValue;

            var preparedDataX = Environment.Utitlity.CalcFrequencyRange(minX, maxX, 8);
            option.XTick = preparedDataX.Step;
            option.XMin = preparedDataX.MinValue;
            option.XMax = preparedDataX.MaxValue;

            option.Points = points;

            return option;
        }

        private MP.MapDrawingDataPoint MakeDrawingPointForStation(double lon, double lat)
        {
            return new MP.MapDrawingDataPoint
            {
                Color = System.Windows.Media.Brushes.Green,
                Fill = System.Windows.Media.Brushes.ForestGreen,
                Location = new Models.Location(lon, lat)
            };
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
        //    var data = new MP.MapDrawingData();
        //    var points = new List<MP.MapDrawingDataPoint>();

        //    if (this.CurrentShortSensor != null)
        //    {
        //        var svcSensor = SVC.SdrnsControllerWcfClient.GetSensorById(this.CurrentShortSensor.Id);
        //        if (svcSensor != null)
        //        {
        //            var modelSensor = Mappers.Map(svcSensor);
        //            if (modelSensor.Locations != null && modelSensor.Locations.Length > 0)
        //            {
        //                var sensorPoints = modelSensor.Locations
        //                    .Where(l => ("A".Equals(l.Status, StringComparison.OrdinalIgnoreCase)
        //                            || "Z".Equals(l.Status, StringComparison.OrdinalIgnoreCase))
        //                            && l.Lon.HasValue
        //                            && l.Lat.HasValue)
        //                    .Select(l => this.MakeDrawingPointForSensor(l.Status, l.Lon.Value, l.Lat.Value))
        //                    .ToArray();

        //                points.AddRange(sensorPoints);
        //            }
        //        }
        //    }

        //    var currentMeasTaskResultStation = this.CurrentResultsMeasurementsStation;
        //    var currentMeasTaskResult = this.CurrentMeasurementResults;
        //    var currentMeasTaskStation = this.CurrentMeasTaskStation;
        //    var currentMeasTask = this.CurrentMeasTask;

        //    // To define station points

        //    if (currentMeasTaskResultStation != null)
        //    {
        //        if (currentMeasTask != null && currentMeasTaskResultStation.StationId.HasValue)
        //        {
        //            var measTaskStations = currentMeasTask.StationsForMeasurements;
        //            if (measTaskStations != null && measTaskStations.Length > 0)
        //            {
        //                var stationForShow = measTaskStations
        //                    .Where(measTaskStation =>
        //                           measTaskStation.IdStation == currentMeasTaskResultStation.StationId.Value
        //                        && measTaskStation.Site != null
        //                        && measTaskStation.Site.Lon.HasValue
        //                        && measTaskStation.Site.Lat.HasValue)
        //                    .FirstOrDefault();

        //                if (stationForShow != null)
        //                {
        //                    points.Add(this.MakeDrawingPointForStation(stationForShow.Site.Lon.Value, stationForShow.Site.Lat.Value));
        //                }
        //            }
        //        }
        //    }
        //    else if (currentMeasTaskResult != null)
        //    {
        //        var measTaskResultStations = currentMeasTaskResult.ResultsMeasStation;
        //        if (measTaskResultStations != null && measTaskResultStations.Length > 0)
        //        {
        //            if (currentMeasTask != null)
        //            {
        //                var measTaskStations = currentMeasTask.StationsForMeasurements;
        //                if (measTaskStations != null && measTaskStations.Length > 0)
        //                {
        //                    var stationsForShow = measTaskStations
        //                        .Where(measTaskStation =>
        //                                measTaskResultStations.Where(s => s.Idstation.HasValue && s.Idstation.Value == measTaskStation.IdStation).FirstOrDefault() != null)
        //                        .ToArray();

        //                    if (stationsForShow.Length > 0)
        //                    {
        //                        var stationPoints = stationsForShow
        //                            .Where(s => s.Site != null && s.Site.Lon.HasValue && s.Site.Lat.HasValue)
        //                            .Select(s => this.MakeDrawingPointForStation(s.Site.Lon.Value, s.Site.Lat.Value))
        //                            .ToArray();

        //                        if (stationPoints.Length > 0)
        //                        {
        //                            points.AddRange(stationPoints);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else if (currentMeasTaskStation != null)
        //    {
        //        if (currentMeasTaskStation.SiteLon.HasValue && currentMeasTaskStation.SiteLat.HasValue)
        //        {
        //            points.Add(this.MakeDrawingPointForStation(currentMeasTaskStation.SiteLon.Value, currentMeasTaskStation.SiteLat.Value));
        //        }
        //    }
        //    else if (currentMeasTask != null)
        //    {
        //        var taskStations = currentMeasTask.StationsForMeasurements;
        //        if (taskStations != null && taskStations.Length > 0)
        //        {
        //            var stationPoints = taskStations
        //                .Where(s => s.Site != null && s.Site.Lon.HasValue && s.Site.Lat.HasValue)
        //                .Select(s => this.MakeDrawingPointForStation(s.Site.Lon.Value, s.Site.Lat.Value))
        //                .ToArray();

        //            if (stationPoints.Length > 0)
        //            {
        //                points.AddRange(stationPoints);
        //            }
        //        }
        //    }


        //    data.Points = points.ToArray();
        //    this.CurrentMapData = data;
        }
    }
}
