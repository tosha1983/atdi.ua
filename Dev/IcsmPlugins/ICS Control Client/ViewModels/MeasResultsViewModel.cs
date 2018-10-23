using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Models;
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
        private MeasurementResultsViewModel _currentMeasurementResult;
        private ShortMeasurementResultsViewModel _currentShortMeasurementResults;
        private ResultsMeasurementsStationViewModel _currentResultsMeasurementsStation;
        private LevelMeasurementsCarViewModel _currentLevelMeasurements;
        private CS.ChartOption _currentChartOption;
        private MP.MapDrawingData _currentMapData;
        private ModelType _currentModel;
        private SDR.MeasurementType _measDtParamTypeMeasurements;
        #endregion

        #region Sources (Adapters)
        public ShortMeasurementResultsDataAdatper ShortMeasResultsSpecial => this._shortMeasResults;
        public ResultsMeasurementsStationDataAdapter ResultsMeasurementsStations => this._resultsMeasurementsStations;
        public LevelMeasurementsCarDataAdapter LevelMeasurements => this._levelMeasurements;
        #endregion

        private ShortMeasurementResultsDataAdatper _shortMeasResults;
        private ResultsMeasurementsStationDataAdapter _resultsMeasurementsStations;
        private LevelMeasurementsCarDataAdapter _levelMeasurements;

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
            this._shortMeasResults = new ShortMeasurementResultsDataAdatper();
            this._resultsMeasurementsStations = new ResultsMeasurementsStationDataAdapter();
            this._levelMeasurements = new LevelMeasurementsCarDataAdapter();
            this._measDtParamTypeMeasurements = SDR.MeasurementType.MonitoringStations;
            this.ReloadShortMeasResult();
        }

        public SDR.MeasurementType MeasDtParamTypeMeasurements
        {
            get => this._measDtParamTypeMeasurements;
            set => this.Set(ref this._measDtParamTypeMeasurements, value, ReloadShortMeasResult);
        }
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
        public MeasurementResultsViewModel CurrentMeasurementResults
        {
            get => this._currentMeasurementResult;
            set => this.Set(ref this._currentMeasurementResult, value, () => { UpdateCurrentChartOption(); this.RedrawMap(); });
        }
        public ShortMeasurementResultsViewModel CurrentShortMeasurementResults
        {
            get => this._currentShortMeasurementResults;
            set => this.Set(ref this._currentShortMeasurementResults, value, () => { ReloadMeasResultDetail(); });
        }
        public ResultsMeasurementsStationViewModel CurrentResultsMeasurementsStation
        {
            get => this._currentResultsMeasurementsStation;
            set => this.Set(ref this._currentResultsMeasurementsStation, value, () => { ReloadLevelMeasurements(); UpdateCurrentChartOption(); });
        }
        public LevelMeasurementsCarViewModel CurrentLevelMeasurements
        {
            get => this._currentLevelMeasurements;
            set => this.Set(ref this._currentLevelMeasurements, value, () => { this.RedrawMap(); });
        }

        private void ReloadShortMeasResult()
        {
            var sdrResult = SVC.SdrnsControllerWcfClient.GetShortMeasResultsSpecial(this._measDtParamTypeMeasurements);
            this._shortMeasResults.Source = sdrResult;
        }

        //private void ReloadShortMeasResults()
        //{
        //    int taskId = 0;
        //    if (this._currentShortMeasTask != null)
        //    {
        //        taskId = this._currentShortMeasTask.Id;
        //    }

        //    var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetShortMeasResultsByTask(taskId);

        //    this._shortMeasResults.Source = sdrMeasResults;
        //    this._resultsMeasurementsStations.Source = null;
        //}

        //private void ResultsMeasurementsStation()
        //{
        //    if (this._currentShortResultsMeasurementsStation != null)
        //    {
        //        int StationId;
        //        int.TryParse(this._currentShortResultsMeasurementsStation.StationId, out StationId);

        //        var measResults = SVC.SdrnsControllerWcfClient.GetResMeasStation(this._currentShortMeasurementResults.MeasSdrResultsId, StationId);

        //        var measResultsViewModel = Mappers.Map(measResults);
        //        this.CurrentResultsMeasurementsStation = measResultsViewModel;
        //    }
        //    else
        //    {
        //        this.CurrentResultsMeasurementsStation = null;
        //        this._resultsMeasurementsStations.Source = null;
        //    }
        //}
        private void ReloadMeasResultDetail()
        {
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
                Location = new Models.Location(lon, lat),
                Opacity = 0.85,
                Width = 10,
                Height = 10
            };
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
            var routes = new List<MP.MapDrawingDataRoute>();
            var polygons = new List<MP.MapDrawingDataPolygon>();
            var points = new List<MP.MapDrawingDataPoint>();

            var sdrRoutes = SVC.SdrnsControllerWcfClient.GetRoutes(this._currentShortMeasurementResults.MeasSdrResultsId);
            if (sdrRoutes != null && sdrRoutes.Length > 0)
            {
                sdrRoutes.ToList().ForEach(sdrRoute =>
                {
                    var routePoints = new List<Location>();
                    if (sdrRoute.RoutePoints != null && sdrRoute.RoutePoints.Length > 0)
                    {
                        sdrRoute.RoutePoints.ToList().ForEach(point =>
                        {
                            routePoints.Add(new Location(point.Lon, point.Lat));
                        });
                    }
                    routes.Add(new MP.MapDrawingDataRoute() { Points = routePoints.ToArray(), Color = System.Windows.Media.Colors.Black, Fill = System.Windows.Media.Colors.Black });
                });
            }

            var sdrPolygonPoints = SVC.SdrnsControllerWcfClient.GetSensorPoligonPoint(this._currentShortMeasurementResults.MeasSdrResultsId);
            if (sdrPolygonPoints != null && sdrPolygonPoints.Length > 0)
            {
                var polygonPoints = new List<Location>();
                sdrPolygonPoints.ToList().ForEach(sdrPolygonPoint =>
                {
                    if (sdrPolygonPoint.Lon.HasValue && sdrPolygonPoint.Lat.HasValue)
                    {
                        polygonPoints.Add(new Location(sdrPolygonPoint.Lon.Value, sdrPolygonPoint.Lat.Value));
                    }
                    polygons.Add(new MP.MapDrawingDataPolygon() { Points = polygonPoints.ToArray(), Color = System.Windows.Media.Colors.Red, Fill = System.Windows.Media.Colors.Black });
                });
            }

            if (this.CurrentLevelMeasurements != null)
            {
                if (this.CurrentLevelMeasurements.Lon.HasValue && this.CurrentLevelMeasurements.Lat.HasValue)
                {
                    System.Windows.Media.Brush pointBrush = System.Windows.Media.Brushes.GreenYellow;
                    if (this.CurrentLevelMeasurements.LeveldBmkVm.HasValue && this.CurrentLevelMeasurements.LeveldBmkVm.Value != 0)
                    {
                        pointBrush = GetBrushColor(10, 80, this.CurrentLevelMeasurements.LeveldBmkVm.Value);
                    }
                    else if (this.CurrentLevelMeasurements.LeveldBm.HasValue && this.CurrentLevelMeasurements.LeveldBm.Value != 0)
                    {
                        pointBrush = GetBrushColor(-100, -30, this.CurrentLevelMeasurements.LeveldBm.Value);
                    }

                    points.Add(new MP.MapDrawingDataPoint() { Location = new Location(this.CurrentLevelMeasurements.Lon.Value, this.CurrentLevelMeasurements.Lat.Value), Opacity = 0.5, Height = 5, Width = 5, Fill = pointBrush, Color = pointBrush });
                }
            }

            var currentMeasTaskResultStation = this.CurrentResultsMeasurementsStation;
            var currentMeasTaskResult = this.CurrentMeasurementResults;
            var currentMeasTask = SVC.SdrnsControllerWcfClient.GetMeasTaskById(CurrentMeasurementResults.MeasTaskId);

            //// To define station points
            if (currentMeasTaskResultStation != null)
            {
                if (currentMeasTask != null && currentMeasTaskResultStation.StationId != null)
                {
                    var measTaskStations = currentMeasTask.StationsForMeasurements;
                    if (measTaskStations != null && measTaskStations.Length > 0)
                    {
                        var stationForShow = measTaskStations
                            .Where(measTaskStation =>
                                   measTaskStation.IdStation.ToString() == currentMeasTaskResultStation.StationId
                                && measTaskStation.Site != null
                                && measTaskStation.Site.Lon.HasValue
                                && measTaskStation.Site.Lat.HasValue)
                            .FirstOrDefault();

                        if (stationForShow != null)
                        {
                            points.Add(this.MakeDrawingPointForStation(stationForShow.Site.Lon.Value, stationForShow.Site.Lat.Value));
                        }
                    }
                }
            }
            else if (currentMeasTaskResult != null)
            {
                var measTaskResultStations = currentMeasTaskResult.ResultsMeasStation;
                if (measTaskResultStations != null && measTaskResultStations.Length > 0)
                {
                    if (currentMeasTask != null)
                    {
                        var measTaskStations = currentMeasTask.StationsForMeasurements;
                        if (measTaskStations != null && measTaskStations.Length > 0)
                        {
                            var stationsForShow = measTaskStations
                                .Where(measTaskStation =>
                                        measTaskResultStations.Where(s => s.Idstation == measTaskStation.IdStation.ToString()).FirstOrDefault() != null)
                                .ToArray();

                            if (stationsForShow.Length > 0)
                            {
                                var stationPoints = stationsForShow
                                    .Where(s => s.Site != null && s.Site.Lon.HasValue && s.Site.Lat.HasValue)
                                    .Select(s => this.MakeDrawingPointForStation(s.Site.Lon.Value, s.Site.Lat.Value))
                                    .ToArray();

                                if (stationPoints.Length > 0)
                                {
                                    points.AddRange(stationPoints);
                                }
                            }
                        }
                    }
                }
            }

            data.Routes = routes.ToArray();
            data.Polygons = polygons.ToArray();
            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }

        private System.Windows.Media.Brush GetBrushColor(double minVal, double maxVal, double val)
        {
            byte id;

            if (val <= minVal)
                id = 0;
            else if (val >= maxVal)
                id = 255;
            else
            {
                double oneprcVal = (maxVal - minVal) / 100;
                double rezVal = (val - minVal) / oneprcVal * 2.55;
                byte.TryParse(Math.Round(rezVal, 0).ToString(), out id);
            }

            byte a;
            byte b;
            byte.TryParse(id.ToString(), out a);
            byte.TryParse((255 - id).ToString(), out b);

            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(a, a, b));
        }
    }
}
