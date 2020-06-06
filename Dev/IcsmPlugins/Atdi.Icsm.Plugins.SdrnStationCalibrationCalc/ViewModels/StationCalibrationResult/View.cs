using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System.Collections.Specialized;
using System.Collections;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using MP = Atdi.WpfControls.EntityOrm.Maps;
using System.Data;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult.Adapters;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult.Queries;
using IC_ES = Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using System.ComponentModel;



namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult
{
    [ViewXaml("StationCalibrationResult.xaml")]
    [ViewCaption("Station Calibration result")]
    public class View : ViewBase
    {
        private DateTime? _dateStartLoadResults;
        private DateTime? _dateStopLoadResults;

        private long _resultId;
        private MP.MapDrawingData _currentMapData;

        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;



        public ViewCommand LoadResultsCommand { get; set; }

        private CalcServerDataLayer _dataLayer { get; set; }

        private IList _currentStationCalibrationResultModel;
        public StationCalibrationResultDataAdapter  StationCalibrationResultDataAdapter { get; set; }

        private IList _currentStationCalibrationDriveTestsModel;
        public StationCalibrationDriveTestsDataAdapter StationCalibrationDriveTestsDataAdapter { get; set; }


        private IList _currentStationCalibrationStaModel;
        public StationCalibrationStaDataAdapter  StationCalibrationStaDataAdapter { get; set; }

        public View(
            CalcServerDataLayer dataLayer,
            IObjectReader objectReader,
            ICommandDispatcher commandDispatcher,
            ViewStarter starter,
            StationCalibrationResultDataAdapter stationCalibrationResultDataAdapter,
            StationCalibrationDriveTestsDataAdapter stationCalibrationDriveTestsDataAdapter,
            StationCalibrationStaDataAdapter stationCalibrationStaDataAdapter,
            IEventBus eventBus,
            ILogger logger)
        {
            _objectReader = objectReader;
            _commandDispatcher = commandDispatcher;
            _starter = starter;
            _eventBus = eventBus;
            _logger = logger;

            this.StationCalibrationResultDataAdapter = stationCalibrationResultDataAdapter;
            this.StationCalibrationDriveTestsDataAdapter = stationCalibrationDriveTestsDataAdapter;
            this.StationCalibrationStaDataAdapter = stationCalibrationStaDataAdapter;


            this.LoadResultsCommand = new ViewCommand(this.OnLoadResultsCommand);

            this._dataLayer = dataLayer;

            this.DateStartLoadResults = DateTime.Now.AddDays(-30);
            this.DateStopLoadResults = DateTime.Now;

            this.RedrawMap();
        }

        public DateTime? DateStartLoadResults
        {
            get => this._dateStartLoadResults;
            set => this.Set(ref this._dateStartLoadResults, value);
        }
        public DateTime? DateStopLoadResults
        {
            get => this._dateStopLoadResults;
            set => this.Set(ref this._dateStopLoadResults, value);
        }

        private void OnLoadResultsCommand(object parameter)
        {
            try
            {
                this.StationCalibrationStaDataAdapter.resultId = -1;
                this.StationCalibrationStaDataAdapter.Refresh();

                this.StationCalibrationDriveTestsDataAdapter.resultId = -1;
                this.StationCalibrationDriveTestsDataAdapter.Refresh();

                this.StationCalibrationResultDataAdapter.resultId = ResultId;
                this.StationCalibrationResultDataAdapter.dateTimeStart = new DateTimeOffset(DateStartLoadResults.Value);
                this.StationCalibrationResultDataAdapter.dateTimeStop = new DateTimeOffset(DateStopLoadResults.Value);
                this.StationCalibrationResultDataAdapter.Refresh();

            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }

        public long ResultId
        {
            get => this._resultId;
            set => this.Set(ref this._resultId, value, () => { this.OnChangeResultId(value); });
        }

        private void OnChangeResultId(long resId)
        {
            this.StationCalibrationResultDataAdapter.resultId = resId;
            this.StationCalibrationResultDataAdapter.Refresh();
        }

        public MP.MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }

        /// <summary>
        /// Координаты станции
        /// </summary>
        private void RedrawMapStationCalibrationStaModel()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();
            var polygonsByStations = new List<MP.MapDrawingDataPolygon>();

            if (this._currentStationCalibrationStaModel != null)
            {
                foreach (StationCalibrationStaModel v in this._currentStationCalibrationStaModel)
                {
                    var sta = GetStationCalibrationStaById(v.Id);
                    if (sta != null)
                    {
                        for (int i = 0; i < sta.Length; i++)
                        {
                            points.Add(new MP.MapDrawingDataPoint()
                            {
                                Location = new MP.Location()
                                {
                                    Lon = sta[i].New_Lon_dec_deg,
                                    Lat = sta[i].New_Lat_dec_deg
                                },
                                Color = System.Windows.Media.Brushes.Blue,
                                Fill = System.Windows.Media.Brushes.DarkBlue,

                                Opacity = 0.85,
                                Width = 10,
                                Height = 10
                            });
                        }
                    }
                }
            }
            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }

        /// <summary>
        /// Маршруты драйв тестов
        /// </summary>
        private void RedrawStationCalibrationDriveTestsModel()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();
            var polygonsByStations = new List<MP.MapDrawingDataPolygon>();

            if (this._currentStationCalibrationDriveTestsModel != null)
            {
                foreach (StationCalibrationDriveTestsModel v in this._currentStationCalibrationDriveTestsModel)
                {
                    if (v.DriveTestId > 0)
                    {
                        var driveTestPoints = GetDriveTestPoints(v.DriveTestId);
                        if (driveTestPoints != null)
                        {
                            for (int i = 0; i < driveTestPoints.Length; i++)
                            {
                                points.Add(new MP.MapDrawingDataPoint()
                                {
                                    Location = new MP.Location()
                                    {
                                        Lat = driveTestPoints[i].Coordinate.Latitude,
                                        Lon = driveTestPoints[i].Coordinate.Longitude
                                    },
                                    Color = System.Windows.Media.Brushes.Orange,
                                    Fill = System.Windows.Media.Brushes.OrangeRed,
                                    Opacity = 0.85,
                                    Width = 4,
                                    Height = 4
                                });
                            }
                        }

                        if (v.LinkToStationMonitoringId > 0)
                        {
                            var sta = GetStationCalibrationStaByResId(v.LinkToStationMonitoringId);
                            if (sta != null)
                            {
                                for (int i = 0; i < sta.Length; i++)
                                {
                                    points.Add(new MP.MapDrawingDataPoint()
                                    {
                                        Location = new MP.Location()
                                        {
                                            Lon = sta[i].New_Lon_dec_deg,
                                            Lat = sta[i].New_Lat_dec_deg
                                        },
                                        Color = System.Windows.Media.Brushes.Blue,
                                        Fill = System.Windows.Media.Brushes.DarkBlue,

                                        Opacity = 0.85,
                                        Width = 10,
                                        Height = 10
                                    });
                                }
                            }
                        }
                    }
                }
            }
            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }

        /// <summary>
        /// Координаты станции и маршруты драйв тестов
        /// </summary>
        private void RedrawMap()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();
            var polygonsByStations = new List<MP.MapDrawingDataPolygon>();

            if (this._currentStationCalibrationResultModel!=null)
            {
                foreach (StationCalibrationResultModel v in this._currentStationCalibrationResultModel)
                {
                    var sta = GetStationCalibrationStaByResId(v.Id);
                    if (sta != null)
                    {
                        for (int i = 0; i < sta.Length; i++)
                        {
                            points.Add(new MP.MapDrawingDataPoint()
                            {
                                Location = new MP.Location()
                                {
                                    Lon = sta[i].New_Lon_dec_deg,
                                    Lat = sta[i].New_Lat_dec_deg
                                },
                                Color = System.Windows.Media.Brushes.Blue,
                                Fill = System.Windows.Media.Brushes.DarkBlue,

                                Opacity = 0.85,
                                Width = 10,
                                Height = 10
                            });
                        }
                    }
                }
            }
            
            //data.Polygons = polygonsByStations.ToArray();
            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }


        public IList CurrentStationCalibrationResultModel
        {
            get { return this._currentStationCalibrationResultModel; }
            set
            {
                this._currentStationCalibrationResultModel = value;

                if (this._currentStationCalibrationResultModel != null)
                {
                    foreach (StationCalibrationResultModel v in this._currentStationCalibrationResultModel)
                    {
                        this.StationCalibrationStaDataAdapter.resultId = v.Id;
                        this.StationCalibrationStaDataAdapter.Refresh();
                        this.StationCalibrationDriveTestsDataAdapter.resultId = v.Id;
                        this.StationCalibrationDriveTestsDataAdapter.Refresh();
                        this.RedrawMap();
                    }
                }
            }
        }


        public IC_ES.DriveTestPoint[] GetDriveTestPoints(long Id)
        {
            var resDriveTestPoints = _objectReader
                .Read<IC_ES.DriveTestPoint[]>()
                .By(new DriveTestPointsResultsModelById()
                {
                    Id = Id
                });
            return resDriveTestPoints;
        }

        public StationCalibrationStaModel[] GetStationCalibrationStaByResId(long ResId)
        {
            var resRoutes = _objectReader
                .Read<StationCalibrationStaModel[]>()
                .By(new StationCalibrationStaModelByResultId()
                {
                    ResultId = ResId
                });
            return resRoutes;
        }

        public StationCalibrationStaModel[] GetStationCalibrationStaById(long Id)
        {
            var resRoutes = _objectReader
                .Read<StationCalibrationStaModel[]>()
                .By(new StationCalibrationStaModelById()
                {
                     Id = Id
                });
            return resRoutes;
        }



        public IList CurrentStationCalibrationDriveTestsModel
        {
            get { return this._currentStationCalibrationDriveTestsModel; }
            set
            {
                this._currentStationCalibrationDriveTestsModel = value;
                this.RedrawStationCalibrationDriveTestsModel();
            }
        }


        public IList CurrentStationCalibrationStaModel
        {
            get { return this._currentStationCalibrationStaModel; }
            set
            {
                this._currentStationCalibrationStaModel = value;
                this.RedrawMapStationCalibrationStaModel();
            }
        }

        

        public override void Dispose()
        {

        }

    }

}
