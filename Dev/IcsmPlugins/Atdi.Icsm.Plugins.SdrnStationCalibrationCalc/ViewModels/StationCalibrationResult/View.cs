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
using System.ComponentModel;



namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult
{
    [ViewXaml("StationCalibrationResult.xaml")]
    [ViewCaption("Station Calibration result")]
    public class View : ViewBase
    {
        private long _resultId;
        private MP.MapDrawingData _currentMapData;

        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

      
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

            this._dataLayer = dataLayer;
            this.RedrawMap();
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
            
            this.StationCalibrationStaDataAdapter.resultId = resId;
            this.StationCalibrationStaDataAdapter.Refresh();

            this.StationCalibrationDriveTestsDataAdapter.resultId = resId;
            this.StationCalibrationDriveTestsDataAdapter.Refresh();
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
                                    Lon = sta[i].New_Lon_deg,
                                    Lat = sta[i].New_Lat_deg
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
                        var routes = GetRoutesByIdStationMonitoring(v.DriveTestId);
                        if (routes != null)
                        {
                            for (int i = 0; i < routes.Length; i++)
                            {
                                points.Add(new MP.MapDrawingDataPoint()
                                {
                                    Location = new MP.Location()
                                    {
                                        Lat = routes[i].Latitude,
                                        Lon = routes[i].Longitude
                                    },
                                    Color = System.Windows.Media.Brushes.Green,
                                    Fill = System.Windows.Media.Brushes.ForestGreen,
                                    Opacity = 0.85,
                                    Width = 10,
                                    Height = 10
                                });
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
                    var sta = GetStationCalibrationStaByResId(v.ResultId);
                    if (sta != null)
                    {
                        for (int i = 0; i < sta.Length; i++)
                        {
                            points.Add(new MP.MapDrawingDataPoint()
                            {
                                Location = new MP.Location()
                                {
                                    Lon = sta[i].New_Lon_deg,
                                    Lat = sta[i].New_Lat_deg
                                },
                                Color = System.Windows.Media.Brushes.Blue,
                                Fill = System.Windows.Media.Brushes.DarkBlue,

                                Opacity = 0.85,
                                Width = 10,
                                Height = 10
                            });
                        }
                    }
                    var resDriveTests = GetDriveTests(v.ResultId);
                    if (resDriveTests != null)
                    {
                        for (int k = 0; k < resDriveTests.Length; k++)
                        {
                            if (resDriveTests[k].DriveTestId > 0)
                            {
                                var routes = GetRoutesByIdStationMonitoring(resDriveTests[k].DriveTestId);
                                for (int i = 0; i < routes.Length; i++)
                                {
                                    points.Add(new MP.MapDrawingDataPoint()
                                    {
                                        Location = new MP.Location()
                                        {
                                            Lat = routes[i].Latitude,
                                            Lon = routes[i].Longitude
                                        },
                                        Color = System.Windows.Media.Brushes.Green,
                                        Fill = System.Windows.Media.Brushes.ForestGreen,
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
                        this.StationCalibrationStaDataAdapter.resultId = v.ResultId;
                        this.StationCalibrationStaDataAdapter.Refresh();
                        this.StationCalibrationDriveTestsDataAdapter.resultId = v.ResultId;
                        this.StationCalibrationDriveTestsDataAdapter.Refresh();
                        this.RedrawMap();
                    }
                }
            }
        }


        public RoutesStationMonitoringModel[] GetRoutesByIdStationMonitoring(long Id)
        {
            var resRoutes = _objectReader
                .Read<RoutesStationMonitoringModel[]>()
                .By(new RoutesStationMonitoringModelById()
                {
                    Id = Id
                });
            return resRoutes;
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


        public StationCalibrationDriveTestsModel[] GetDriveTests(long ResId)
        {
            var resRoutes = _objectReader
                .Read<StationCalibrationDriveTestsModel[]>()
                .By(new StationCalibrationDriveTestsModelById()
                {
                      ResultId = ResId
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
