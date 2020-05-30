using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System.Collections.Specialized;
using System.Collections;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Adapters;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using MP = Atdi.WpfControls.EntityOrm.Maps;
using System.Data;
using System.Windows;
using WPF =  System.Windows.Controls;





namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager
{

    [ViewXaml("StationCalibrationManager.xaml")]
    [ViewCaption("Station Calibration calc client")]
    public class View : ViewBase
    {
        private bool _isEnabledStart = false;
        private bool _isEnabledFieldId = false;

        private DateTime? _dateStartLoadDriveTest;
        private DateTime? _dateStopLoadDriveTest;

        private long _taskId;

        private MP.MapDrawingData _currentMapData;
        private IList _currentAreas;


        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        private IcsmStationName _selectedIcsmStationName;
        private SelectedStationType _selectedStationType;
        private MethodParamsCalculationModel _methodParamsCalculationModel;


        private ParamsCalculationModel _currentParamsCalculationModel;
        private StationMonitoringModel _currentStationMonitoringModel;
        private GetStationsParamsModel _currentGetStationsParameters;



        private AreasDataAdapter AreasDataAdapter;
        public StationMonitoringDataAdapter StationMonitoringDataAdapter { get; set; }
        public ParametersDataAdapter ParametersDataAdapter { get; set; }


        public ViewCommand StartStationCalibrationCommand { get; set; }
        public ViewCommand LoadDriveTestsCommand { get; set; }






        private CalcServerDataLayer _dataLayer { get; set; }


        public View(
            CalcServerDataLayer dataLayer,
            ParametersDataAdapter parametersDataAdapter,
            StationMonitoringDataAdapter stationMonitoringDataAdapter,
            IObjectReader objectReader,
            ICommandDispatcher commandDispatcher,
            ViewStarter starter,
            IEventBus eventBus,
            ILogger logger)
        {
            _objectReader = objectReader;
            _commandDispatcher = commandDispatcher;
            _starter = starter;
            _eventBus = eventBus;
            _logger = logger;


            this._dataLayer = dataLayer;


            this.StartStationCalibrationCommand = new ViewCommand(this.OnStartStationCalibrationCommand);
            this.LoadDriveTestsCommand = new ViewCommand(this.OnLoadDriveTestsCommand);

            this._currentParamsCalculationModel = new ParamsCalculationModel();
            this._currentGetStationsParameters = new GetStationsParamsModel();

            this.AreasDataAdapter = new AreasDataAdapter();
            this.ParametersDataAdapter = parametersDataAdapter;
            this.StationMonitoringDataAdapter = stationMonitoringDataAdapter;



            this.ReloadData();
            this.RedrawMap();

            
        }

        public bool IsEnabledStart
        {
            get => this._isEnabledStart;
            set => this.Set(ref this._isEnabledStart, value);
        }

        public bool IsEnabledFieldId
        {
            get => this._isEnabledFieldId;
            set => this.Set(ref this._isEnabledFieldId, value);
        }

        public long TaskId
        {
            get => this._taskId;
            set => this.Set(ref this._taskId, value, () => { this.OnChangedTaskIdParams(value); });
        }

        private void OnChangedTaskIdParams(long taskId)
        {
            this._currentParamsCalculationModel = ReadParamsCalculationByTaskId(this._taskId);
            if (this._currentParamsCalculationModel.Method != null)
            {
                MethodParamsCalculationModelVal = (MethodParamsCalculationModel)this._currentParamsCalculationModel.Method;
            }
        }
   

        private void CheckEnabledStart()
        {
            if (CurrentAreas.Count == 0)
            {
                IsEnabledStart = false;
            }
            else
            {
                IsEnabledStart = true;
            }
        }

        public AreasDataAdapter Areas => this.AreasDataAdapter;

        public MP.MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }

        public SelectedStationType SelectedStationTypeVal
        {
            get => this._selectedStationType;
            set => this.Set(ref this._selectedStationType, value);
        }

        public MethodParamsCalculationModel MethodParamsCalculationModelVal
        {
            get => this._methodParamsCalculationModel;
            set => this.Set(ref this._methodParamsCalculationModel, value);
        }

        public IcsmStationName SelectedIcsmStationNameVal
        {
            get => this._selectedIcsmStationName;
            set => this.Set(ref this._selectedIcsmStationName, value);
        }


        public IList<IcsmStationName> SelectedIcsmStationNameValues => Enum.GetValues(typeof(IcsmStationName)).Cast<IcsmStationName>().ToList();
        public IList<SelectedStationType> SelectedStationTypeValues => Enum.GetValues(typeof(SelectedStationType)).Cast<SelectedStationType>().ToList();
        public IList<MethodParamsCalculationModel> MethodParamsCalculationModelValues => Enum.GetValues(typeof(MethodParamsCalculationModel)).Cast<MethodParamsCalculationModel>().ToList();


        public static void CalcCoordinateDistance(double LongitudeSource, double LatitudeSource, out double LongitudeOut, out double LatitudeOut, double distance_km, TypeCoord typeCoord)
        {
            LongitudeOut = 0;
            LatitudeOut = 0;
            var ym = Math.Cos(LongitudeSource) * 111000.0 * LatitudeSource;
            var xm = LongitudeSource * 111000;
            var distance_m = distance_km * 1000;

            if (typeCoord == TypeCoord.RecalcLatitude)
            {
                var yRecalc = ym + distance_m;
                LongitudeOut = xm / 111000;
                LatitudeOut = yRecalc / (Math.Cos(LongitudeSource) * 111000.0);
            }
            if (typeCoord == TypeCoord.RecalcLongitude)
            {
                var xRecalc = xm + distance_m;
                LongitudeOut = xRecalc / 111000;
                LatitudeOut = LatitudeSource;
            }
        }

        private void RedrawMap()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();
            var polygons = new List<MP.MapDrawingDataPolygon>();



            if (CurrentStationMonitoringModel != null)
            {
                var routes = GetRoutesByIdStationMonitoring(CurrentStationMonitoringModel.Id);
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

            if (this._currentAreas != null)
                foreach (AreaModel area in this._currentAreas)
                {
                    if (area.Location != null)
                    {
                        var dataLocationModels = new DataLocationModel[area.Location.Length];
                        for (int i=0;i< area.Location.Length; i++)
                        {
                            dataLocationModels[i] = new DataLocationModel()
                            {
                                Longitude = ICSM.IMPosition.Dms2Dec(area.Location[i].Longitude),
                                Latitude = ICSM.IMPosition.Dms2Dec(area.Location[i].Latitude)
                            };

                        }

                        var polygonPoints = new List<MP.Location>();
                        var polygonPointsAnother = new List<MP.Location>();

                        var lX = new List<double>();
                        var lY = new List<double>();

                        var minX = dataLocationModels.Select(x => x.Longitude).Min();
                        var maxX = dataLocationModels.Select(x => x.Longitude).Max();
                        var minY = dataLocationModels.Select(x => x.Latitude).Min();
                        var maxY = dataLocationModels.Select(x => x.Latitude).Max();



                        CalcCoordinateDistance(minX, maxY, out double ouX1, out double ouY1, -CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLongitude);
                        CalcCoordinateDistance(minX, maxY, out double ouX2, out double ouY2, -CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLatitude);

                        CalcCoordinateDistance(maxX, maxY, out double ouX3, out double ouY3, CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLongitude);
                        CalcCoordinateDistance(maxX, maxY, out double ouX4, out double ouY4, -CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLatitude);

                        CalcCoordinateDistance(maxX, minY, out double ouX5, out double ouY5, CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLongitude);
                        CalcCoordinateDistance(maxX, minY, out double ouX6, out double ouY6, CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLatitude);

                        CalcCoordinateDistance(minX, minY, out double ouX7, out double ouY7, -CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLongitude);
                        CalcCoordinateDistance(minX, minY, out double ouX8, out double ouY8, CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLatitude);


                        lX.Add(ouX1);
                        lX.Add(ouX3);
                        lX.Add(ouX5);
                        lX.Add(ouX7);

                        lY.Add(ouY2);
                        lY.Add(ouY4);
                        lY.Add(ouY6);
                        lY.Add(ouY8);


                        //area.ExternalContour = new DataLocationModel[4];
                        //area.ExternalContour[0] = new DataLocationModel()
                        //{
                        //    Longitude = lX.Min(),
                        //    Latitude = lY.Min()
                        //};
                        //area.ExternalContour[1] = new DataLocationModel()
                        //{
                        //    Longitude = lX.Max(),
                        //    Latitude = lY.Min()
                        //};
                        //area.ExternalContour[2] = new DataLocationModel()
                        //{
                        //    Longitude = lX.Max(),
                        //    Latitude = lY.Max()
                        //};
                        //area.ExternalContour[3] = new DataLocationModel()
                        //{
                        //    Longitude = lX.Min(),
                        //    Latitude = lY.Max()
                        //};


                        //polygonPointsAnother.Add(new MP.Location()
                        //{
                        //    Lon = lX.Min(),
                        //    Lat = lY.Min()
                        //});

                        //polygonPointsAnother.Add(new MP.Location()
                        //{
                        //    Lon = lX.Max(),
                        //    Lat = lY.Min()
                        //});

                        //polygonPointsAnother.Add(new MP.Location()
                        //{
                        //    Lon = lX.Max(),
                        //    Lat = lY.Max()
                        //});

                        //polygonPointsAnother.Add(new MP.Location()
                        //{
                        //    Lon = lX.Min(),
                        //    Lat = lY.Max()
                        //});


                        area.ExternalContour = new DataLocationModel[4];
                        area.ExternalContour[0] = new DataLocationModel()
                        {
                            Longitude = lX.Min(),
                            Latitude = lY.Max()
                        };
                        area.ExternalContour[1] = new DataLocationModel()
                        {
                            Longitude = lX.Max(),
                            Latitude = lY.Max()
                        };
                        area.ExternalContour[2] = new DataLocationModel()
                        {
                            Longitude = lX.Max(),
                            Latitude = lY.Min()
                        };
                        area.ExternalContour[3] = new DataLocationModel()
                        {
                            Longitude = lX.Min(),
                            Latitude = lY.Min()
                        };


                        polygonPointsAnother.Add(new MP.Location()
                        {
                            Lon = lX.Min(),
                            Lat = lY.Max()
                        });

                        polygonPointsAnother.Add(new MP.Location()
                        {
                            Lon = lX.Max(),
                            Lat = lY.Max()
                        });

                        polygonPointsAnother.Add(new MP.Location()
                        {
                            Lon = lX.Max(),
                            Lat = lY.Min()
                        });

                        polygonPointsAnother.Add(new MP.Location()
                        {
                            Lon = lX.Min(),
                            Lat = lY.Min()
                        });



                        polygons.Add(new MP.MapDrawingDataPolygon()
                        {
                            Points = polygonPointsAnother.ToArray(),
                            Color = System.Windows.Media.Colors.Aqua,
                            Fill = System.Windows.Media.Colors.Aqua
                        });

                        foreach (var point in dataLocationModels)
                        {
                            polygonPoints.Add(new MP.Location()
                            {
                                Lon = point.Longitude,
                                Lat = point.Latitude
                            });
                        }
                        polygons.Add(new MP.MapDrawingDataPolygon() {
                            Points = polygonPoints.ToArray(),
                            Color = System.Windows.Media.Colors.Red,
                            Fill = System.Windows.Media.Colors.Red
                        });
                    }
                }

            data.Polygons = polygons.ToArray();
            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }

        public DateTime? DateStartLoadDriveTest
        {
            get => this._dateStartLoadDriveTest;
            set => this.Set(ref this._dateStartLoadDriveTest, value);
        }
        public DateTime? DateStopLoadDriveTest
        {
            get => this._dateStopLoadDriveTest;
            set => this.Set(ref this._dateStopLoadDriveTest, value);
        }

        public IList CurrentAreas
        {
            get => this._currentAreas;
            set
            {
                this._currentAreas = value;
                if (CurrentParamsCalculation.DistanceAroundContour_km == null)
                {
                    CurrentParamsCalculation.DistanceAroundContour_km = 0;
                }
                RedrawMap();
                CheckEnabledStart();
            }
        }

        public StationMonitoringModel CurrentStationMonitoringModel
        {
            get => this._currentStationMonitoringModel;
            set => this.Set(ref this._currentStationMonitoringModel, value, () => { this.OnChangedCurrentStationMonitoringModel(value); });
        }

        




        private void OnChangedCurrentStationMonitoringModel(StationMonitoringModel stationMonitoringModel)
        {
            RedrawMap();
        }

        public GetStationsParamsModel GetStationsParams
        {
            get => this._currentGetStationsParameters;
            set => this.Set(ref this._currentGetStationsParameters, value, () => { this.OnChangedGetStationsParams(value); });
        }

        private void OnChangedGetStationsParams(GetStationsParamsModel getStationsParamsModel)
        {

        }

        public ParamsCalculationModel CurrentParamsCalculation
        {
            get => this._currentParamsCalculationModel;
            set => this.Set(ref this._currentParamsCalculationModel, value, () => { this.OnChangedParamsCalculation(value); });
        }


        private void OnChangedParamsCalculation(ParamsCalculationModel paramsCalculationModel)
        {

        }

        public ParamsCalculationModel ReadParamsCalculationByTaskId(long taskId)
        {
            var resMeas = _objectReader
                .Read<ParamsCalculationModel>()
                .By(new GetParamsCalculationByTaskId()
                {
                     TaskId = taskId
                });
            return resMeas;
        }

        public StationMonitoringModel ReadStationMonitoring(long id)
        {
            var resMeas = _objectReader
                .Read<StationMonitoringModel>()
                .By(new StationMonitoringModelById()
                {
                    Id = id
                });
            return resMeas;
        }

        public GCIDDataModel ReadGCIDDataModel(string licenseGsid, string regionCode, string standard)
        {
            var resGCID = _objectReader
                .Read<GCIDDataModel>()
                .By(new GCIDDataModelByParams()
                {
                    LicenseGsid = licenseGsid,
                    RegionCode = regionCode,
                    Standard = standard
                });
            return resGCID;
        }

        public IcsmMobStation[] ReadStations()
        {
            AreaModel[] selectedAreaModels = new AreaModel[CurrentAreas.Count];
            int index = 0;
            foreach (AreaModel areaModel in CurrentAreas)
            {
                selectedAreaModels[index] = areaModel;
                index++;
            }
            var resStations = _objectReader
           .Read<IcsmMobStation[]>()
           .By(new MobStationsLoadModelByParams()
           {
               Standard = GetStationsParams.Standard,
               IdentifierStation = GetStationsParams.Id,
               StatusForActiveStation = GetStationsParams.StateForActiveStation,
               StatusForNotActiveStation = GetStationsParams.StateForNotActiveStation,
               TableName = SelectedIcsmStationNameVal.ToString().ToUpper(),
               AreaModel = selectedAreaModels,
               SelectedStationType = SelectedStationTypeVal
           });
            return resStations;
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



        private void ReloadData()
        {
            this.DateStartLoadDriveTest = DateTime.Now.AddDays(-30);
            this.DateStopLoadDriveTest = DateTime.Now;
            this.AreasDataAdapter.Refresh();
        }



        public override void Dispose()
        {

        }


        private void OnLoadDriveTestsCommand(object parameter)
        {
            this.StationMonitoringDataAdapter.StartDateTime = this.DateStartLoadDriveTest.Value;
            this.StationMonitoringDataAdapter.StopDateTime = this.DateStopLoadDriveTest.Value;
            this.StationMonitoringDataAdapter.Refresh();
        }

        private void OnStartStationCalibrationCommand(object parameter)
        {

            if (((CurrentAreas!=null) && (CurrentAreas.Count == 0))  || (CurrentAreas==null))
            {
                System.Windows.Forms.MessageBox.Show("Please select area!");
                return;
            }
            if ((CurrentParamsCalculation.DistanceAroundContour_km == null) || ((CurrentParamsCalculation.DistanceAroundContour_km !=null) && (CurrentParamsCalculation.DistanceAroundContour_km ==0)))
            {
                System.Windows.Forms.MessageBox.Show("Please fill parameter 'DistanceAroundContour_km'");
                return;
            }
            if (string.IsNullOrEmpty(GetStationsParams.StateForActiveStation))
            {
                System.Windows.Forms.MessageBox.Show("Please fill 'StateForActiveStation'");
                return;
            }
            if (string.IsNullOrEmpty(GetStationsParams.StateForNotActiveStation))
            {
                System.Windows.Forms.MessageBox.Show("Please fill 'StateForNotActiveStation'");
                return;
            }
            if ((GetStationsParams.Id == null) && (SelectedStationTypeVal== SelectedStationType.OneStation))
            {
                System.Windows.Forms.MessageBox.Show("Please fill 'Id'");
                return;
            }
            if (string.IsNullOrEmpty(GetStationsParams.Standard))
            {
                System.Windows.Forms.MessageBox.Show("Please fill 'Standard'");
                return;
            }

            var stations = ReadStations();
            if (stations.Length > 0)
            {
                this._currentParamsCalculationModel.InfocMeasResults = new long[1] { CurrentStationMonitoringModel.Id };
                this._currentParamsCalculationModel.StationIds = stations.Select(x => Convert.ToInt64(x.ExternalCode)).ToArray();
                StationCalibrationCalcTask.Run(this._dataLayer.Origin, this._dataLayer.Executor, stations, this._currentParamsCalculationModel, TaskId);
            }
            else
            {
                System.Windows.MessageBox.Show("No stations with suitable parameters!");
            }
        }

    }
    public enum TypeCoord
    {
        RecalcLatitude,
        RecalcLongitude,
    }

}
