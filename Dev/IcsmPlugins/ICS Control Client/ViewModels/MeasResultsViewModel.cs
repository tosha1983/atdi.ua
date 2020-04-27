using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Models;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;
using XICSM.ICSControlClient.Models.WcfDataApadters;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using CS = XICSM.ICSControlClient.WpfControls.Charts;
using MP = XICSM.ICSControlClient.WpfControls.Maps;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Windows;
using FRM = System.Windows.Forms;
using FM = XICSM.ICSControlClient.Forms;
using ICSM;
using System.Globalization;
using TR = System.Threading;

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

        #region Commands
        public WpfCommand GetCSVCommand { get; set; }
        public WpfCommand SearchStationCommand { get; set; }
        public WpfCommand PrevSpecCommand { get; set; }
        public WpfCommand NextSpecCommand { get; set; }
        public WpfCommand FilterApplyCommand { get; set; }
        #endregion

        #region Current Objects
        private MeasurementResultsViewModel _currentMeasurementResults;
        private ResultsMeasurementsStationViewModel _currentResultsMeasurementsStation;
        private ResultsMeasurementsStationViewModel _currentResultsMeasurementsStationData;
        private LevelMeasurementsCarViewModel _currentLevelMeasurements;
        private GeneralResultViewModel _currentGeneralResult;
        private CS.ChartOption _currentChartOption;
        private MP.MapDrawingData _currentMapData;
        private ModelType _currentModel;
        private SDR.MeasurementType _measDtParamTypeMeasurements;
        private ResultsMeasurementsStationFilters _currentStationsGolbalFilter;
        private bool _taskOnlyChecked;
        private bool _sensorOnlyChecked;
        private Visibility _resultStationsVisibility = Visibility.Hidden;
        private Visibility _resFreq1Visibility = Visibility.Hidden;
        private Visibility _resFreq2Visibility = Visibility.Hidden;
        private Visibility _resIdStationVisibility = Visibility.Hidden;
        private Visibility _resSpecVisibility = Visibility.Hidden;
        private Visibility _resTimeMeasVisibility = Visibility.Hidden;
        private Visibility _resLevelMes1Visibility = Visibility.Hidden;
        #endregion

        #region Sources (Adapters)
        public MeasurementResultsDataAdatper ShortMeasResultsSpecial => this._measResults;
        public ResultsMeasurementsStationDataAdapter ResultsMeasurementsStations => this._resultsMeasurementsStations;
        public LevelMeasurementsCarDataAdapter LevelMeasurements => this._levelMeasurements;
        public GeneralResultDataAdapter GeneralResults => this._generalResults;
        #endregion

        private MeasurementResultsDataAdatper _measResults;
        private ResultsMeasurementsStationDataAdapter _resultsMeasurementsStations;
        private LevelMeasurementsCarDataAdapter _levelMeasurements;
        private GeneralResultDataAdapter _generalResults;

        private double? _LowFreq;
        private double? _UpFreq;
        private string _specLabelText = "0 of 0";
        private int _selectedSpectrum = 0;

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
            this._currentStationsGolbalFilter = new ResultsMeasurementsStationFilters();
            this._measResults = new MeasurementResultsDataAdatper();
            this._resultsMeasurementsStations = new ResultsMeasurementsStationDataAdapter();
            this._levelMeasurements = new LevelMeasurementsCarDataAdapter();
            this._generalResults = new GeneralResultDataAdapter();
            this._measDtParamTypeMeasurements = SDR.MeasurementType.MonitoringStations;
            this.GetCSVCommand = new WpfCommand(this.OnGetCSVCommand);
            this.SearchStationCommand = new WpfCommand(this.OnSearchStationCommand);
            this.PrevSpecCommand = new WpfCommand(this.OnPrevSpecCommand);
            this.NextSpecCommand = new WpfCommand(this.OnNextSpecCommand);
            this.FilterApplyCommand = new WpfCommand(this.OnFilterApplyCommand);
            this.LoadSettings();
            this.ReloadMeasResult();
        }
        public SDR.MeasurementType MeasDtParamTypeMeasurements
        {
            get => this._measDtParamTypeMeasurements;
            set => this.Set(ref this._measDtParamTypeMeasurements, value, ReloadMeasResult);
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
        public ResultsMeasurementsStationFilters CurrentStationsGolbalFilter
        {
            get => this._currentStationsGolbalFilter;
            set => this.Set(ref this._currentStationsGolbalFilter, value);
        }
        public MeasurementResultsViewModel CurrentMeasurementResults
        {
            get => this._currentMeasurementResults;
            set => this.Set(ref this._currentMeasurementResults, value, () => { ReloadMeasResaltDetail(); UpdateCurrentChartOption(); RedrawMap(); });
        }
        public GeneralResultViewModel CurrentGeneralResult
        {
            get => this._currentGeneralResult;
            set => this.Set(ref this._currentGeneralResult, value, () => { UpdateCurrentChartOption(); RedrawMap(); });
        }
        public ResultsMeasurementsStationViewModel CurrentResultsMeasurementsStation
        {
            get => this._currentResultsMeasurementsStation;
            set => this.Set(ref this._currentResultsMeasurementsStation, value, () => { ReloadMeasResultStationDetail(); ReloadLevelMeasurements(); UpdateCurrentChartOption(); RedrawMap(); });
        }
        public ResultsMeasurementsStationViewModel CurrentResultsMeasurementsStationData
        {
            get => this._currentResultsMeasurementsStationData;
            set => this.Set(ref this._currentResultsMeasurementsStationData, value);
        }
        public LevelMeasurementsCarViewModel CurrentLevelMeasurements
        {
            get => this._currentLevelMeasurements;
            set => this.Set(ref this._currentLevelMeasurements, value);
        }
        public Visibility ResultStationsVisibility
        {
            get => this._resultStationsVisibility;
            set => this.Set(ref this._resultStationsVisibility, value);
        }
        public Visibility ResFreq1Visibility
        {
            get => this._resFreq1Visibility;
            set => this.Set(ref this._resFreq1Visibility, value);
        }
        public Visibility ResFreq2Visibility
        {
            get => this._resFreq2Visibility;
            set => this.Set(ref this._resFreq2Visibility, value);
        }
        public Visibility ResIdStationVisibility
        {
            get => this._resIdStationVisibility;
            set => this.Set(ref this._resIdStationVisibility, value);
        }
        public Visibility ResSpecVisibility
        {
            get => this._resSpecVisibility;
            set => this.Set(ref this._resSpecVisibility, value);
        }
        public Visibility ResTimeMeasVisibility
        {
            get => this._resTimeMeasVisibility;
            set => this.Set(ref this._resTimeMeasVisibility, value);
        }
        public Visibility ResLevelMes1Visibility
        {
            get => this._resLevelMes1Visibility;
            set => this.Set(ref this._resLevelMes1Visibility, value);
        }
        public double? LowFreq
        {
            get => this._LowFreq;
            set => this.Set(ref this._LowFreq, value);
        }
        public double? UpFreq
        {
            get => this._UpFreq;
            set => this.Set(ref this._UpFreq, value);
        }
        public string SpecLabelText
        {
            get => this._specLabelText;
            set => this.Set(ref this._specLabelText, value);
        }
        private void ReloadMeasResult()
        {
            var sdrResult = SVC.SdrnsControllerWcfClient.GetMeasResultsHeaderSpecial(this._measDtParamTypeMeasurements);
            this._measResults.Source = sdrResult.OrderByDescending(c => c.Id.MeasSdrResultsId).ToArray();

            if (this._measDtParamTypeMeasurements == SDR.MeasurementType.MonitoringStations)
            {
                this.ResFreq1Visibility = Visibility.Collapsed;
                this.ResFreq2Visibility = Visibility.Visible;
                this.ResIdStationVisibility = Visibility.Visible;
                this.ResSpecVisibility = Visibility.Visible;
                this.ResTimeMeasVisibility = Visibility.Visible;
                this.ResLevelMes1Visibility = Visibility.Visible;
            }
            else if (this._measDtParamTypeMeasurements == SDR.MeasurementType.SpectrumOccupation)
            {
                this.ResFreq1Visibility = Visibility.Visible;
                this.ResFreq2Visibility = Visibility.Collapsed;
                this.ResIdStationVisibility = Visibility.Collapsed;
                this.ResSpecVisibility = Visibility.Collapsed;
                this.ResTimeMeasVisibility = Visibility.Collapsed;
                this.ResLevelMes1Visibility = Visibility.Collapsed;
            }
            else if (this._measDtParamTypeMeasurements == SDR.MeasurementType.Level)
            {
                this.ResFreq1Visibility = Visibility.Visible;
                this.ResFreq2Visibility = Visibility.Collapsed;
                this.ResIdStationVisibility = Visibility.Collapsed;
                this.ResSpecVisibility = Visibility.Collapsed;
                this.ResTimeMeasVisibility = Visibility.Visible;
                this.ResLevelMes1Visibility = Visibility.Collapsed;
            }
            else if (this._measDtParamTypeMeasurements == SDR.MeasurementType.Signaling)
            {
                this.ResFreq1Visibility = Visibility.Visible;
                this.ResFreq2Visibility = Visibility.Collapsed;
                this.ResIdStationVisibility = Visibility.Collapsed;
                this.ResSpecVisibility = Visibility.Collapsed;
                this.ResTimeMeasVisibility = Visibility.Collapsed;
                this.ResLevelMes1Visibility = Visibility.Collapsed;
            }
            else
            {
                this.ResFreq1Visibility = Visibility.Visible;
                this.ResFreq2Visibility = Visibility.Visible;
                this.ResIdStationVisibility = Visibility.Visible;
                this.ResSpecVisibility = Visibility.Visible;
                this.ResTimeMeasVisibility = Visibility.Visible;
                this.ResLevelMes1Visibility = Visibility.Visible;
            }

            if (this._measDtParamTypeMeasurements == SDR.MeasurementType.MonitoringStations)
            {
                this.ResultStationsVisibility = Visibility.Visible;
            }
            else
            {
                this.ResultStationsVisibility = Visibility.Hidden;
            }
        }

        public bool TaskOnlyChecked
        {
            get => this._taskOnlyChecked;
            set => this.Set(ref this._taskOnlyChecked, value, () => { ApplyFilterResultByTask(); });
        }
        public bool SensorOnlyChecked
        {
            get => this._sensorOnlyChecked;
            set => this.Set(ref this._sensorOnlyChecked, value, () => { ApplyFilterResultBySensor(); });
        }
        private void ApplyFilterResultByTask()
        {
            var id = this._currentMeasurementResults.MeasTaskId;
            if (this._taskOnlyChecked)
            {
                this._measResults.ApplyFilter(c => c.MeasTaskId == id);
            }
            else
            {
                this._measResults.ClearFilter();
            }
        }
        private void ApplyFilterResultBySensor()
        {
            var id = this._currentMeasurementResults.SensorTechId;
            if (this._sensorOnlyChecked)
            {
                this._measResults.ApplyFilter(c => c.SensorTechId == id);
            }
            else
            {
                this._measResults.ClearFilter();
            }
        }
        private void ReloadMeasResaltDetail()
        {
            if (this._currentMeasurementResults != null)
            {
                if (this._measDtParamTypeMeasurements == SDR.MeasurementType.MonitoringStations)
                {
                    //var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetResMeasStationHeaderByResId(this._currentMeasurementResults.MeasSdrResultsId);
                    //this._resultsMeasurementsStations.Source = sdrMeasResults.OrderByDescending(c => c.Id).ToArray();
                    var filter = new SDR.ResultsMeasurementsStationFilters()
                    {
                        FreqBg = PluginHelper.ConvertStringToDouble(this._currentStationsGolbalFilter.FreqBg),
                        FreqEd = PluginHelper.ConvertStringToDouble(this._currentStationsGolbalFilter.FreqEd),
                        MeasGlobalSid = this._currentStationsGolbalFilter.MeasGlobalSid,
                        Standard = this._currentStationsGolbalFilter.Standard
                    };
                    var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetResMeasStationHeaderByResId(this._currentMeasurementResults.MeasSdrResultsId, filter);
                    this._resultsMeasurementsStations.Source = sdrMeasResults;
                }
                else
                {
                    var sdrMeasResultsDetail = SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(this._currentMeasurementResults.MeasSdrResultsId, null, null);
                    LowFreq = sdrMeasResultsDetail.FrequenciesMeasurements == null ? (double?)null : (sdrMeasResultsDetail.FrequenciesMeasurements.Length == 0 ? 0 : sdrMeasResultsDetail.FrequenciesMeasurements.Min(f => f.Freq));
                    UpFreq = sdrMeasResultsDetail.FrequenciesMeasurements == null ? (double?)null : (sdrMeasResultsDetail.FrequenciesMeasurements.Length == 0 ? 0 : sdrMeasResultsDetail.FrequenciesMeasurements.Max(f => f.Freq));
                }
            }
            else
            {
                this.CurrentMeasurementResults = null;
                this._resultsMeasurementsStations.Source = null;
                LowFreq = null;
                UpFreq = null;
            }
            this.CurrentGeneralResult = null;
            this.CurrentResultsMeasurementsStationData = null;
        }
        private void ReloadMeasResultStationDetail()
        {
            if (this._currentResultsMeasurementsStation != null)
            {
                var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetResMeasStationById(this._currentResultsMeasurementsStation.Id);
                this.CurrentResultsMeasurementsStationData = Mappers.Map(sdrMeasResults);
                this._selectedSpectrum = 0;
                this.GenerateSpecLabelText();
                if (this.CurrentResultsMeasurementsStationData != null && this.CurrentResultsMeasurementsStationData.GeneralResults != null && this.CurrentResultsMeasurementsStationData.GeneralResults.Length > 0)
                {
                    this.CurrentGeneralResult = Mappers.Map(this.CurrentResultsMeasurementsStationData.GeneralResults[0]);
                }
                else
                    this.CurrentGeneralResult = null;
            }
        }
        private void ReloadLevelMeasurements()
        {
            if (this.CurrentResultsMeasurementsStation == null)
            {
                this._levelMeasurements.Source = null;
                return;
            }
            this._levelMeasurements.Source = this._currentResultsMeasurementsStationData.LevelMeasurements;
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
                XTick = 10,
                IsEnableSaveToFile  = true
            };

            var generalResult = this._currentGeneralResult;
            if (generalResult == null)
                return option;

            var spectrumLevels = generalResult.LevelsSpecrum;
            if (spectrumLevels == null || spectrumLevels.Length == 0)
                return option;

            if (!generalResult.SpecrumStartFreq.HasValue || !generalResult.SpecrumSteps.HasValue)
                return option;

            var count = spectrumLevels.Length;
            var points = new Point[count];
            var linesList = new List<CS.ChartLine>();
            var maxX = default(double);
            var minX = default(double);

            var maxY = default(double);
            var minY = default(double);
            for (int i = 0; i < count; i++)
            {
                var valX = Convert.ToDouble(generalResult.SpecrumStartFreq + i * generalResult.SpecrumSteps / 1000);
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

            if (generalResult.T1.GetValueOrDefault(0) != 0)
            {
                linesList.Add(new CS.ChartLine() { Point = new Point { X = generalResult.T1.Value, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true, Name = "T1", LabelLeft = 5, LabelTop = -25 });
            }
            if (generalResult.MarkerIndex.GetValueOrDefault(0) != 0)
            {
                linesList.Add(new CS.ChartLine() { Point = new Point { X = generalResult.MarkerIndex.Value, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true, Name = "M", LabelLeft = 5, LabelTop = -35 });
            }
            if (generalResult.T2.GetValueOrDefault(0) != 0)
            {
                linesList.Add(new CS.ChartLine() { Point = new Point { X = generalResult.T2.Value, Y = 0 }, LineColor = System.Windows.Media.Brushes.DarkRed, IsHorizontal = false, IsVertical = true, Name = "T2", LabelLeft = 5, LabelTop = -45 });
            }

            var preparedDataY = Environment.Utitlity.CalcLevelRange(minY, maxY);
            option.YTick = 10;
            option.YMax = preparedDataY.MaxValue;
            option.YMin = preparedDataY.MinValue;

            var preparedDataX = Environment.Utitlity.CalcFrequencyRange(minX, maxX, 8);
            option.XMin = preparedDataX.MinValue;
            option.XMax = preparedDataX.MaxValue;
            if (minX != maxX)
                option.XTick = preparedDataX.Step;

            option.Points = points;
            option.LinesArray = linesList.ToArray();
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
                XTick = 10,
                IsEnableSaveToFile = true
            };

            var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(result.MeasSdrResultsId, null, null);

            var count = sdrMeasResults.FrequenciesMeasurements.Length;
            var points = new Point[count];
            var max = default(double);
            var min = default(double);
            for (int i = 0; i < count; i++)
            {
                var ms = sdrMeasResults.MeasurementsResults[i] as SDR.SpectrumOccupationMeasurementResult;
                var valX = sdrMeasResults.FrequenciesMeasurements[i].Freq;
                var valY = ms.Value ?? 0;
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
                XTick = 10,
                IsEnableSaveToFile = true
            };

            var sdrMeasResults = SVC.SdrnsControllerWcfClient.GetMeasurementResultByResId(result.MeasSdrResultsId, null, null);

            var count = sdrMeasResults.FrequenciesMeasurements.Length;
            var points = new Point[count];

            var maxX = default(double);
            var minX = default(double);

            var maxY = default(double);
            var minY = default(double);

            for (int i = 0; i < count; i++)
            {
                var ms = sdrMeasResults.MeasurementsResults[i] as SDR.LevelMeasurementResult;
                var valX = sdrMeasResults.FrequenciesMeasurements[i].Freq;
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

            //var preparedDataX = Environment.Utitlity.CalcFrequencyRange(minX, maxX, 8);
            //option.XTick = preparedDataX.Step;
            //option.XMin = preparedDataX.MinValue;
            //option.XMax = preparedDataX.MaxValue;

            var preparedDataX = Environment.Utitlity.CalcLevelRange(minX - 5, maxX + 5);
            option.XTick = 50;
            option.XMin = preparedDataX.MinValue;
            option.XMax = preparedDataX.MaxValue;

            option.Points = points;

            return option;
        }
        private void RedrawMap()
        {
            var data = new MP.MapDrawingData();
            var routes = new List<MP.MapDrawingDataRoute>();
            var polygons = new List<MP.MapDrawingDataPolygon>();
            var points = new List<MP.MapDrawingDataPoint>();

            var sdrRoutes = SVC.SdrnsControllerWcfClient.GetRoutes(this._currentMeasurementResults.MeasSdrResultsId);
            if (sdrRoutes != null && sdrRoutes.Length > 0)
            {
                var routePoints = new List<Location>();
                sdrRoutes.ToList().ForEach(sdrRoute =>
                {
                    if (sdrRoute.RoutePoints != null && sdrRoute.RoutePoints.Length > 0)
                    {
                        sdrRoute.RoutePoints.OrderBy(c => c.StartTime).ToList().ForEach(point =>
                        {
                            routePoints.Add(new Location(point.Lon, point.Lat));
                        });
                    }
                });
                routes.Add(new MP.MapDrawingDataRoute() { Points = routePoints.ToArray(), Color = System.Windows.Media.Colors.Black, Fill = System.Windows.Media.Colors.Black });
            }

            var sdrPolygonPoints = SVC.SdrnsControllerWcfClient.GetSensorPoligonPoint(this._currentMeasurementResults.MeasSdrResultsId);
            if (sdrPolygonPoints != null && sdrPolygonPoints.Length > 0)
            {
                var polygonPoints = new List<Location>();
                sdrPolygonPoints.ToList().ForEach(sdrPolygonPoint =>
                {
                    if (sdrPolygonPoint.Lon.HasValue && sdrPolygonPoint.Lat.HasValue)
                    {
                        polygonPoints.Add(new Location(sdrPolygonPoint.Lon.Value, sdrPolygonPoint.Lat.Value));
                    }
                });
                polygons.Add(new MP.MapDrawingDataPolygon() { Points = polygonPoints.ToArray(), Color = System.Windows.Media.Colors.Red, Fill = System.Windows.Media.Colors.Red });
            }

            //if (this.CurrentLevelMeasurements != null)
            //{
            //    if (this.CurrentLevelMeasurements.Lon.HasValue && this.CurrentLevelMeasurements.Lat.HasValue)
            //    {
            //        System.Windows.Media.Brush pointBrush = System.Windows.Media.Brushes.GreenYellow;
            //        if (this.CurrentLevelMeasurements.LeveldBmkVm.HasValue && this.CurrentLevelMeasurements.LeveldBmkVm.Value != 0)
            //        {
            //            pointBrush = GetBrushColor(10, 80, this.CurrentLevelMeasurements.LeveldBmkVm.Value);
            //        }
            //        else if (this.CurrentLevelMeasurements.LeveldBm.HasValue && this.CurrentLevelMeasurements.LeveldBm.Value != 0)
            //        {
            //            pointBrush = GetBrushColor(-100, -30, this.CurrentLevelMeasurements.LeveldBm.Value);
            //        }

            //        points.Add(new MP.MapDrawingDataPoint() { Location = new Location(this.CurrentLevelMeasurements.Lon.Value, this.CurrentLevelMeasurements.Lat.Value), Opacity = 0.5, Height = 5, Width = 5, Fill = pointBrush, Color = pointBrush });
            //    }
            //}
            if (this.LevelMeasurements != null)
            {
                foreach (var levelMeasurement in LevelMeasurements.Source)
                {
                    if (levelMeasurement.Lon.HasValue && levelMeasurement.Lat.HasValue)
                    {
                        System.Windows.Media.Brush pointBrush = System.Windows.Media.Brushes.GreenYellow;
                        if (levelMeasurement.LeveldBmkVm.HasValue && levelMeasurement.LeveldBmkVm.Value != 0)
                        {
                            pointBrush = GetBrushColor(10, 80, levelMeasurement.LeveldBmkVm.Value);
                        }
                        else if (levelMeasurement.LeveldBm.HasValue && levelMeasurement.LeveldBm.Value != 0)
                        {
                            pointBrush = GetBrushColor(-100, -30, levelMeasurement.LeveldBm.Value);
                        }
                        points.Add(new MP.MapDrawingDataPoint() { Location = new Location(levelMeasurement.Lon.Value, levelMeasurement.Lat.Value), Opacity = 0.5, Height = 5, Width = 5, Fill = pointBrush, Color = pointBrush });
                    }
                }
            }

            var currentMeasTaskResultStation = this.CurrentResultsMeasurementsStation;
            var currentMeasTaskResult = this.CurrentMeasurementResults;
            var currentMeasTask = SVC.SdrnsControllerWcfClient.GetMeasTaskHeaderById(CurrentMeasurementResults.MeasTaskId);

            //// To define station points
            if (currentMeasTaskResultStation != null)
            {
                if (currentMeasTask != null && currentMeasTaskResultStation.StationId != null)
                {
                    //var measTaskStations = currentMeasTask.StationsForMeasurements;
                    var measTaskStations = SVC.SdrnsControllerWcfClient.GetStationDataForMeasurementsByTaskId(currentMeasTask.Id.Value);
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
                            points.Add(MapsDrawingHelper.MakeDrawingPointForStation(stationForShow.Site.Lon.Value, stationForShow.Site.Lat.Value));
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
                        //var measTaskStations = currentMeasTask.StationsForMeasurements;
                        var measTaskStations = SVC.SdrnsControllerWcfClient.GetStationDataForMeasurementsByTaskId(currentMeasTask.Id.Value);
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
                                    .Select(s => MapsDrawingHelper.MakeDrawingPointForStation(s.Site.Lon.Value, s.Site.Lat.Value))
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

            byte.TryParse(id.ToString(), out byte a);
            byte.TryParse((255 - id).ToString(), out byte b);

            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(a, a, b));
        }
        private void OnGetCSVCommand(object parameter)
        {
            try
            {
                string filename = "";
                if (this._currentMeasurementResults == null || this._currentResultsMeasurementsStationData == null)
                    return;

                int recCount = LevelMeasurements.Source.Count();
                if (recCount == 0)
                {
                    MessageBox.Show("No data for export.");
                    return;
                }

                long taskId = this._currentMeasurementResults.MeasTaskId;
                string stationId = this._currentResultsMeasurementsStationData.StationId;

                FRM.SaveFileDialog sfd = new FRM.SaveFileDialog()
                {
                    Filter = "CSV (*.csv)|*.csv",
                    FileName = "FS_Meas_Res_" + taskId.ToString() + "_" + stationId + ".csv"
                };
                if (sfd.ShowDialog() == FRM.DialogResult.OK)
                {
                    //MessageBox.Show("Data will be exported and you will be notified when it is ready.");
                    if (File.Exists(filename))
                    {
                        try
                        {
                            File.Delete(filename);
                        }
                        catch (IOException ex)
                        {
                            MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                        }
                    }

                    string[] output = new string[recCount + 1];
                    output[0] += "Lon;Lat;Level";

                    for (int i = 0; i < recCount; i++)
                    {
                        var ms = LevelMeasurements.Source[i] as SDR.LevelMeasurementsCar;
                        double leveldBmkVm = 0;
                        if ((!ms.LeveldBmkVm.HasValue || ms.LeveldBmkVm == 0 || ms.LeveldBmkVm == -1 || ms.LeveldBmkVm <= -30 || ms.LeveldBmkVm >= 200))
                        {
                            double freq = 0;
                            if (this._currentGeneralResult.CentralFrequencyMeas.HasValue && this._currentGeneralResult.CentralFrequencyMeas > 0.01)
                            {
                                freq = this._currentGeneralResult.CentralFrequencyMeas.Value;
                            }
                            if (this._currentGeneralResult.CentralFrequency.HasValue && this._currentGeneralResult.CentralFrequency > 0.01)
                            {
                                freq = this._currentGeneralResult.CentralFrequency.Value;
                            }
                            if (freq > 0 && ms.LeveldBm.HasValue && ms.LeveldBm > -300 && ms.LeveldBm < -10)
                            {
                                leveldBmkVm = (float)(77.2 + 20 * Math.Log10(freq) + ms.LeveldBm);
                            }
                        }
                        else
                        {
                            if (ms.LeveldBmkVm.HasValue)
                                leveldBmkVm = ms.LeveldBmkVm.Value;
                        }
                        if (leveldBmkVm > 0)
                            output[i + 1] += ms.Lon.ToString() + ";" + ms.Lat.ToString() + ";" + leveldBmkVm.ToString() + ";";
                    }
                    System.IO.File.WriteAllLines(sfd.FileName, output, System.Text.Encoding.UTF8);
                    MessageBox.Show("Your file was generated and its ready for use.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnSearchStationCommand(object parameter)
        {
            try
            {
                if (this._currentResultsMeasurementsStationData == null || string.IsNullOrEmpty(this._currentResultsMeasurementsStationData.MeasGlobalSID) || this._currentResultsMeasurementsStationData.MeasGlobalSID.Length < 5)
                    return;

                //string stationName = this._currentResultsMeasurementsStationData.MeasGlobalSID;
                string stationName = this._currentResultsMeasurementsStationData.StationId.PadLeft(4, '0');
                double? lonMin = null;
                double? lonMax = null;
                double? latMin = null;
                double? latMax = null;
                decimal? freq = null;

                //stationName = stationName.Substring(stationName.Length - 5, 4).TrimStart('0');

                if (LevelMeasurements.Source == null)
                {
                    MessageBox.Show("No Level Measurements data.");
                    return;
                }
                int recCount = LevelMeasurements.Source.Count();

                if (recCount == 0)
                {
                    MessageBox.Show("No Level Measurements data.");
                    return;
                }

                for (int i = 0; i < recCount; i++)
                {
                    var ms = LevelMeasurements.Source[i] as SDR.LevelMeasurementsCar;

                    if (ms.Lon.HasValue)
                    {
                        if (!lonMin.HasValue || lonMin > ms.Lon)
                            lonMin = ms.Lon;
                        if (!lonMax.HasValue || lonMax < ms.Lon)
                            lonMax = ms.Lon;
                    }

                    if (ms.Lat.HasValue)
                    {
                        if (!latMin.HasValue || latMin > ms.Lat)
                            latMin = ms.Lat;
                        if (!latMax.HasValue || latMax < ms.Lat)
                            latMax = ms.Lat;
                    }

                    if (!freq.HasValue)
                        freq = ms.CentralFrequency;
                }

                var source = new IMRecordset("MOB_STATION", IMRecordset.Mode.ReadOnly);
                source.Select("ID,NAME,Position.LONGITUDE,Position.LATITUDE");
                source.SetWhere("NAME", IMRecordset.Operation.Eq, stationName);
                if (lonMin.HasValue)
                    source.SetWhere("Position.LONGITUDE", IMRecordset.Operation.Gt, lonMin.Value - 0.7);
                if (lonMax.HasValue)
                    source.SetWhere("Position.LONGITUDE", IMRecordset.Operation.Lt, lonMax.Value + 0.7);
                if (latMin.HasValue)
                    source.SetWhere("Position.LATITUDE", IMRecordset.Operation.Gt, latMin.Value - 0.7);
                if (latMax.HasValue)
                    source.SetWhere("Position.LATITUDE", IMRecordset.Operation.Lt, latMax.Value + 0.7);

                Dictionary<int, int> stations = new Dictionary<int, int>();

                for (source.Open(); !source.IsEOF(); source.MoveNext())
                {
                    var rs = new IMRecordset("MOBSTA_FREQS", IMRecordset.Mode.ReadOnly);
                    rs.Select("ID,STA_ID,TX_FREQ");
                    rs.SetWhere("STA_ID", IMRecordset.Operation.Eq, source.GetI("ID"));

                    double? minFreq = null;
                    double? maxFreq = null;

                    for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
                    {
                        var frq = rs.GetD("TX_FREQ");
                        if (frq != IM.NullD)
                        {
                            if (!minFreq.HasValue || minFreq > frq)
                                minFreq = frq;
                            if (!maxFreq.HasValue || maxFreq < frq)
                                maxFreq = frq;
                        }
                    }

                    if (minFreq.HasValue && maxFreq.HasValue && minFreq - 0.2 < (double)freq && (double)freq < maxFreq + 0.2)
                        stations.Add(source.GetI("ID"), source.GetI("ID"));

                    //MessageBox.Show(source.GetS("ID") + " - " + source.GetS("NAME") + "(" + source.GetD("Position.LONGITUDE").ToString() + ":" + source.GetD("Position.LATITUDE").ToString() + ")");
                }

                if (stations.Count() > 0)
                {
                    var dlgForm = new FM.StationListForm() { stationIDs = string.Join(",", stations.Keys.ToArray()) };
                    dlgForm.ShowDialog();
                    dlgForm.Dispose();
                }
                else
                {
                    MessageBox.Show("Stations not found");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnPrevSpecCommand(object parameter)
        {
            if (this.CurrentResultsMeasurementsStationData != null && this.CurrentResultsMeasurementsStationData.GeneralResults != null && this.CurrentResultsMeasurementsStationData.GeneralResults.Length > 0 && _selectedSpectrum > 0)
            {
                this.CurrentGeneralResult = Mappers.Map(this.CurrentResultsMeasurementsStationData.GeneralResults[--_selectedSpectrum]);
                this.GenerateSpecLabelText();
            }
        }
        private void OnNextSpecCommand(object parameter)
        {
            if (this.CurrentResultsMeasurementsStationData != null && this.CurrentResultsMeasurementsStationData.GeneralResults != null && this.CurrentResultsMeasurementsStationData.GeneralResults.Length > 0 && _selectedSpectrum < this.CurrentResultsMeasurementsStationData.GeneralResults.Length - 1)
            {
                this.CurrentGeneralResult = Mappers.Map(this.CurrentResultsMeasurementsStationData.GeneralResults[++_selectedSpectrum]);
                this.GenerateSpecLabelText();
            }
        }
        private void OnFilterApplyCommand(object parameter)
        {
            Properties.Settings.Default.GlobalFilterMeasGlobalSid = this._currentStationsGolbalFilter.MeasGlobalSid;
            Properties.Settings.Default.GlobalFilterStandard = this._currentStationsGolbalFilter.Standard;
            Properties.Settings.Default.GlobalFilterFreqBg = this._currentStationsGolbalFilter.FreqBg;
            Properties.Settings.Default.GlobalFilterFreqEd = this._currentStationsGolbalFilter.FreqEd;
            Properties.Settings.Default.Save();
            ReloadMeasResaltDetail();
        }
        private void LoadSettings()
        {
            this._currentStationsGolbalFilter.MeasGlobalSid = Properties.Settings.Default.GlobalFilterMeasGlobalSid;
            this._currentStationsGolbalFilter.Standard = Properties.Settings.Default.GlobalFilterStandard;
            this._currentStationsGolbalFilter.FreqBg = Properties.Settings.Default.GlobalFilterFreqBg;
            this._currentStationsGolbalFilter.FreqEd = Properties.Settings.Default.GlobalFilterFreqEd;
        }
        private void GenerateSpecLabelText()
        {
            if (this.CurrentResultsMeasurementsStationData != null && this.CurrentResultsMeasurementsStationData.GeneralResults != null && this.CurrentResultsMeasurementsStationData.GeneralResults.Length > 0)
                SpecLabelText = (_selectedSpectrum + 1).ToString() + " of " + this.CurrentResultsMeasurementsStationData.GeneralResults.Length.ToString();
            else
                SpecLabelText = "0 of 0";
        }
    }
}
