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
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Modifiers;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using MP = Atdi.WpfControls.EntityOrm.Controls;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using System.Data;
using System.Windows;
using WPF =  System.Windows.Controls;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.Properties;
using MG = Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration;



namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager
{

    [ViewXaml("StationCalibrationManager.xaml")]
    [ViewCaption("Station Calibration calc client")]
    public class View : ViewBase
    {
        private bool _isEnabledStart = false;

        private DateTime? _dateStartLoadDriveTest;
        private DateTime? _dateStopLoadDriveTest;

        private long _taskId;

        private readonly ViewStarter _viewStarter;
        private MP.MapDrawingData _currentMapData;
        private IList _currentAreas;

        private bool _isVisibleFieldId;
        private bool _isVisibleFieldStateForActiveStation;
        private bool _isVisibleFieldStateForNotActiveStation;
        private bool _isVisibleFieldStandard;


        private IEventHandlerToken<Events.OnEditParamsCalculation> _onEditParamsCalculationToken;
        private IEventHandlerToken<Events.OnSavedStations> _onSavedStationsToken;
        private IEventHandlerToken<Events.OnCreatePropagationModels> _onCreatePropagationModelsToken;
        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;
        private readonly AppComponentConfig _appComponentConfig;

        private IcsmStationName _selectedIcsmStationName;
        private SelectedStationType _selectedStationType;
        private MethodParamsCalculationModel _methodParamsCalculationModel;
        private readonly ITransformation _transformation;


        private ParamsCalculationModel _currentParamsCalculationModel;
        private IList _currentStationMonitoringModel;
        private GetStationsParamsModel _currentGetStationsParameters;



        private AreasDataAdapter AreasDataAdapter;
        public StationMonitoringDataAdapter StationMonitoringDataAdapter { get; set; }
        public ParametersDataAdapter ParametersDataAdapter { get; set; }

        public ViewCommand SaveAndStartStationCalibrationCommand { get; set; }
        public ViewCommand SaveStationCalibrationCommand { get; set; }
        public ViewCommand LoadDriveTestsCommand { get; set; }


        private CalcServerDataLayer _dataLayer { get; set; }


        public View(
            ViewStarter viewStarter,
            CalcServerDataLayer dataLayer,
            ParametersDataAdapter parametersDataAdapter,
            StationMonitoringDataAdapter stationMonitoringDataAdapter,
            AppComponentConfig appComponentConfig,
            IObjectReader objectReader,
            ICommandDispatcher commandDispatcher,
            ITransformation transformation,
            ViewStarter starter,
            IEventBus eventBus,
            ILogger logger)
        {
            _objectReader = objectReader;
            _commandDispatcher = commandDispatcher;
            _starter = starter;
            _eventBus = eventBus;
            _logger = logger;
            _viewStarter = viewStarter;


            this._dataLayer = dataLayer;
            this._appComponentConfig = appComponentConfig;


            this.SaveAndStartStationCalibrationCommand = new ViewCommand(this.OnSaveAndStartStationCalibrationCommand);
            this.SaveStationCalibrationCommand = new ViewCommand(this.OnSaveStationCalibrationCommand);
            this.LoadDriveTestsCommand = new ViewCommand(this.OnLoadDriveTestsCommand);

            this._currentParamsCalculationModel = new ParamsCalculationModel();
            this._currentGetStationsParameters = new GetStationsParamsModel();

            this.AreasDataAdapter = new AreasDataAdapter();
            this.ParametersDataAdapter = parametersDataAdapter;
            this.StationMonitoringDataAdapter = stationMonitoringDataAdapter;
            this._transformation = transformation;

            this._onEditParamsCalculationToken = _eventBus.Subscribe<Events.OnEditParamsCalculation>(this.OnEditParamsCalculationsHandle);
            this._onCreatePropagationModelsToken = _eventBus.Subscribe<Events.OnCreatePropagationModels>(this.OnCreatePropagationModelsHandle);
            this._onSavedStationsToken = _eventBus.Subscribe<Events.OnSavedStations>(this.OnSavedStationsHandle);

            if (this._selectedStationType == SelectedStationType.OneStation)
            {
                IsVisibleFieldId = true;
                IsVisibleFieldStandard = false;
                IsVisibleFieldStateForActiveStation = false;
                IsVisibleFieldStateForNotActiveStation = false;
            }
            else
            {
                IsVisibleFieldId = false;
                IsVisibleFieldStandard = true;
                IsVisibleFieldStateForActiveStation = true;
                IsVisibleFieldStateForNotActiveStation = true;
            }

            this.ReloadData();
            this.RedrawMap();

            this.StationMonitoringDataAdapter.StartDateTime = DateTime.Now.AddDays(1);
            this.StationMonitoringDataAdapter.StopDateTime = DateTime.Now.AddDays(1);
            this.StationMonitoringDataAdapter.Refresh();
        }

        public bool IsEnabledStart
        {
            get => this._isEnabledStart;
            set => this.Set(ref this._isEnabledStart, value);
        }

        public bool IsVisibleFieldId
        {
            get => this._isVisibleFieldId;
            set => this.Set(ref this._isVisibleFieldId, value);
        }


        public bool IsVisibleFieldStateForActiveStation
        {
            get => this._isVisibleFieldStateForActiveStation;
            set => this.Set(ref this._isVisibleFieldStateForActiveStation, value);
        }

        public bool IsVisibleFieldStateForNotActiveStation
        {
            get => this._isVisibleFieldStateForNotActiveStation;
            set => this.Set(ref this._isVisibleFieldStateForNotActiveStation, value);
        }

        public bool IsVisibleFieldStandard
        {
            get => this._isVisibleFieldStandard;
            set => this.Set(ref this._isVisibleFieldStandard, value);
        }

        public long TaskId
        {
            get => this._taskId;
            set => this.Set(ref this._taskId, value, () => { this.OnChangedTaskIdParams(value); });
        }

        private void OnChangedTaskIdParams(long taskId)
        {
            this._currentParamsCalculationModel = ReadParamsCalculationByTaskId(this._taskId);
            if ((this._currentParamsCalculationModel.CorrelationThresholdHard == null) && (this._currentParamsCalculationModel.CorrelationThresholdWeak == null) || (this._currentParamsCalculationModel.DistanceAroundContour_km == null))
            {
                FillParametersDefault(this._currentParamsCalculationModel);
            }
            if (this._currentParamsCalculationModel.Method == null)
            {
                MethodParamsCalculationModelVal = MethodParamsCalculationModel.ExhaustiveSearch;
            }
            else
            {
                MethodParamsCalculationModelVal = (MethodParamsCalculationModel)this._currentParamsCalculationModel.Method;
            }
        }

        private void FillParametersDefault(ParamsCalculationModel paramsCalculationModel)
        {
            paramsCalculationModel.CorrelationThresholdWeak = 15;
            paramsCalculationModel.CorrelationThresholdHard = 50;
            paramsCalculationModel.TrustOldResults = true;
            paramsCalculationModel.UseMeasurementSameGSID = false;
            paramsCalculationModel.DistanceAroundContour_km = 5;

            paramsCalculationModel.MinNumberPointForCorrelation = 25;
            paramsCalculationModel.MinRangeMeasurements_dBmkV = -200;
            paramsCalculationModel.MaxRangeMeasurements_dBmkV = 200;
            paramsCalculationModel.CorrelationDistance_m = 20;
            paramsCalculationModel.Delta_dB = 6;
            paramsCalculationModel.MaxAntennasPatternLoss_dB = 30;
            paramsCalculationModel.AltitudeStation = true;
            paramsCalculationModel.ShiftAltitudeStationMin_m = -20;
            paramsCalculationModel.ShiftAltitudeStationMax_m = 20;
            paramsCalculationModel.ShiftAltitudeStationStep_m = 5;
            paramsCalculationModel.MaxDeviationAltitudeStation_m = 10;
            paramsCalculationModel.TiltStation = true;
            paramsCalculationModel.MaxDeviationTiltStation_deg = 3;
            paramsCalculationModel.ShiftTiltStationMin_deg = -20;
            paramsCalculationModel.ShiftTiltStationMax_deg = 0;
            paramsCalculationModel.ShiftTiltStationStep_deg = 1;
            paramsCalculationModel.AzimuthStation = true;
            paramsCalculationModel.ShiftAzimuthStationMin_deg = -180;
            paramsCalculationModel.ShiftAzimuthStationMax_deg = 180;
            paramsCalculationModel.ShiftAzimuthStationStep_deg = 10;
            paramsCalculationModel.MaxDeviationAzimuthStation_deg = 30;
            paramsCalculationModel.CoordinatesStation = true;
            paramsCalculationModel.ShiftCoordinatesStation_m = 100;
            paramsCalculationModel.ShiftCoordinatesStationStep_m = 10;
            paramsCalculationModel.MaxDeviationCoordinatesStation_m = 30;
            paramsCalculationModel.PowerStation = true;
            paramsCalculationModel.ShiftPowerStationMin_dB = -20;
            paramsCalculationModel.ShiftPowerStationMax_dB = 20;
            paramsCalculationModel.ShiftPowerStationStep_dB = 1;
            paramsCalculationModel.CascadeTuning = true;
            paramsCalculationModel.NumberCascade = 2;
            paramsCalculationModel.DetailOfCascade = 5;
            paramsCalculationModel.Method = 0;
        }


        private void CheckEnabledStart()
        {
            if (((CurrentAreas == null) || ((CurrentAreas != null) && (CurrentAreas.Count == 0)))
                || ((CurrentStationMonitoringModel == null) || ((CurrentStationMonitoringModel != null) && (CurrentStationMonitoringModel.Count == 0))))
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
            get { return this._selectedStationType; }
            set
            {
                this.Set(ref this._selectedStationType, value);
                if (this._selectedStationType == SelectedStationType.OneStation)
                {
                    IsVisibleFieldId = true;
                    IsVisibleFieldStandard = false;
                    IsVisibleFieldStateForActiveStation = false;
                    IsVisibleFieldStateForNotActiveStation = false;
                }
                else
                {
                    IsVisibleFieldId = false;
                    IsVisibleFieldStandard = true;
                    IsVisibleFieldStateForActiveStation = true;
                    IsVisibleFieldStateForNotActiveStation = true;
                }
            }
        }

        public MethodParamsCalculationModel MethodParamsCalculationModelVal
        {
            get { return this._methodParamsCalculationModel; }
            set
            {
                this._methodParamsCalculationModel = value;
                this._currentParamsCalculationModel.Method = (byte)this._methodParamsCalculationModel;
            }
        }

        public IcsmStationName SelectedIcsmStationNameVal
        {
            get => this._selectedIcsmStationName;
            set => this.Set(ref this._selectedIcsmStationName, value);
        }


        public IList<IcsmStationName> SelectedIcsmStationNameValues => Enum.GetValues(typeof(IcsmStationName)).Cast<IcsmStationName>().ToList();
        public IList<SelectedStationType> SelectedStationTypeValues => Enum.GetValues(typeof(SelectedStationType)).Cast<SelectedStationType>().ToList();
        public IList<MethodParamsCalculationModel> MethodParamsCalculationModelValues => Enum.GetValues(typeof(MethodParamsCalculationModel)).Cast<MethodParamsCalculationModel>().ToList();


        public Wgs84Coordinate CalcCoordinateDistance(double LongitudeSource, double LatitudeSource, double distance_km, TypeCoord typeCoord)
        {
            var wgs84Coordinate = new Wgs84Coordinate();
            var siteEPSG = this._transformation.ConvertCoordinateToEpgs(new Wgs84Coordinate { Latitude = LatitudeSource, Longitude = LongitudeSource }, 31466);
            if (typeCoord == TypeCoord.RecalcLatitude)
            {
                if (distance_km != 0)
                {
                    siteEPSG.Y = siteEPSG.Y + distance_km * 1000;
                    wgs84Coordinate = this._transformation.ConvertCoordinateToWgs84(siteEPSG, 31466);
                }
                else
                {
                    wgs84Coordinate = new Wgs84Coordinate { Latitude = LatitudeSource, Longitude = LongitudeSource };
                }
            }
            if (typeCoord == TypeCoord.RecalcLongitude)
            {
                if (distance_km != 0)
                {
                    siteEPSG.X = siteEPSG.X + distance_km * 1000;
                    wgs84Coordinate = this._transformation.ConvertCoordinateToWgs84(siteEPSG, 31466);
                }
                else
                {
                    wgs84Coordinate = new Wgs84Coordinate { Latitude = LatitudeSource, Longitude = LongitudeSource };
                }
            }
            return wgs84Coordinate;
        }


        private void RedrawMap()
        {
            try
            {
                var data = new MP.MapDrawingData();
                var points = new List<MP.MapDrawingDataPoint>();
                var polygons = new List<MP.MapDrawingDataPolygon>();

                if (this._currentStationMonitoringModel != null)
                {
                    foreach (StationMonitoringModel model in this._currentStationMonitoringModel)
                    {
                        var routes = GetRoutesByIdStationMonitoring(model.Id);
                        if ((routes != null) && (routes.Length > 0))
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
                                    Color = System.Windows.Media.Brushes.Orange,
                                    Fill = System.Windows.Media.Brushes.OrangeRed,
                                    Opacity = 0.85,
                                    Width = 4,
                                    Height = 4
                                });
                            }
                        }
                    }
                }

                if (this._currentAreas != null)
                    foreach (AreaModel area in this._currentAreas)
                    {
                        if ((area.Location != null) && (area.Location.Length > 0))
                        {
                            var dataLocationModels = new DataLocationModel[area.Location.Length];
                            for (int i = 0; i < area.Location.Length; i++)
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


                            var valX1 = CalcCoordinateDistance(minX, maxY, -CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLongitude);
                            var valY1 = CalcCoordinateDistance(minX, maxY, CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLatitude);

                            var valX2 = CalcCoordinateDistance(maxX, maxY, CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLongitude);
                            var valY2 = CalcCoordinateDistance(maxX, maxY, CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLatitude);

                            var valX3 = CalcCoordinateDistance(maxX, minY, CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLongitude);
                            var valY3 = CalcCoordinateDistance(maxX, minY, -CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLatitude);

                            var valX4 = CalcCoordinateDistance(minX, minY, -CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLongitude);
                            var valY4 = CalcCoordinateDistance(minX, minY, -CurrentParamsCalculation.DistanceAroundContour_km.Value, TypeCoord.RecalcLatitude);


                            lX.Add(valX1.Longitude);
                            lX.Add(valY1.Longitude);
                            lX.Add(valX2.Longitude);
                            lX.Add(valY2.Longitude);
                            lX.Add(valX3.Longitude);
                            lX.Add(valY3.Longitude);
                            lX.Add(valX4.Longitude);
                            lX.Add(valY4.Longitude);


                            lY.Add(valX1.Latitude);
                            lY.Add(valY1.Latitude);
                            lY.Add(valX2.Latitude);
                            lY.Add(valY2.Latitude);
                            lY.Add(valX3.Latitude);
                            lY.Add(valY3.Latitude);
                            lY.Add(valX4.Latitude);
                            lY.Add(valY4.Latitude);

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
                            polygons.Add(new MP.MapDrawingDataPolygon()
                            {
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
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
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
            get { return this._currentAreas; }
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

        public IList CurrentStationMonitoringModel
        {
            get { return this._currentStationMonitoringModel; }
            set
            {
                this._currentStationMonitoringModel = value;
                RedrawMap();

            }
        }

        /// <summary>
        /// Максимально допустимое число точек в drive тесте для заданного в файле конфигурации стандарта
        /// </summary>
        /// <param name="standard"></param>
        /// <returns></returns>
        public int GetMaximumCountPointsInDriveTests(string standard)
        {
            int? maximumCountPoints = null;
            if ((standard == "GSM") || (standard == "GSM-900") || (standard == "GSM-1800") || (standard == "E-GSM"))
            {
                maximumCountPoints = _appComponentConfig.MaximumCountPointsInDriveTestsFor_GSM;
            }
            else if ((standard == "UMTS") || (standard == "WCDMA"))
            {
                maximumCountPoints = _appComponentConfig.MaximumCountPointsInDriveTestsFor_UMTS;
            }
            else if ((standard == "LTE") || (standard == "LTE-1800") || (standard == "LTE-2600") || (standard == "LTE-900"))
            {
                maximumCountPoints = _appComponentConfig.MaximumCountPointsInDriveTestsFor_LTE;
            }
            else if ((standard == "CDMA") || (standard == "CDMA-450") || (standard == "CDMA-800") || (standard == "EVDO"))
            {
                maximumCountPoints = _appComponentConfig.MaximumCountPointsInDriveTestsFor_CDMA;
            }
            else if ((standard == "802.11") || (standard == "ШР"))
            {
                maximumCountPoints = _appComponentConfig.MaximumCountPointsInDriveTestsFor_802_11;
            }
            else
            {
                throw new Exception($"The parameter 'maximum count points' in the config is not defined for the '{standard}' standard");
            }
            return maximumCountPoints.Value;
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

        public IcsmMobStation[] ReadStations(out string standard)
        {
            var selectedAreaModels = new List<AreaModel>();

            foreach (AreaModel areaModel in CurrentAreas)
            {
                if ((areaModel.Location != null) && (areaModel.Location.Length > 0))
                {
                    selectedAreaModels.Add(areaModel);
                }
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
               AreaModel = selectedAreaModels.ToArray(),
               SelectedStationType = SelectedStationTypeVal
           });
            if ((resStations != null) && (resStations.Length > 0))
            {
                standard = resStations[0].Standard;
            }
            else
            {
                standard = string.Empty;
            }
            return resStations;
        }

        public RoutesStationMonitoringModel[] GetRoutesByIdStationMonitoring(long Id)
        {
            RoutesStationMonitoringModel[] resRoutes = null;
            try
            {
                resRoutes = _objectReader
               .Read<RoutesStationMonitoringModel[]>()
               .By(new RoutesStationMonitoringModelById()
               {
                   Id = Id
               });
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
            return resRoutes;
        }



        private void ReloadData()
        {
            try
            {
                this.DateStartLoadDriveTest = DateTime.Now.AddDays(-30);
                this.DateStopLoadDriveTest = DateTime.Now;
                this.AreasDataAdapter.Refresh();
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }



        public override void Dispose()
        {
            _onCreatePropagationModelsToken?.Dispose();
            _onCreatePropagationModelsToken = null;
            _onEditParamsCalculationToken?.Dispose();
            _onEditParamsCalculationToken = null;
            _onSavedStationsToken?.Dispose();
            _onSavedStationsToken = null;
        }


        private void OnLoadDriveTestsCommand(object parameter)
        {
            try
            {
                this.StationMonitoringDataAdapter.StartDateTime = this.DateStartLoadDriveTest.Value;
                this.StationMonitoringDataAdapter.StopDateTime = this.DateStopLoadDriveTest.Value;
                this.StationMonitoringDataAdapter.Refresh();
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }

        private void OnSaveAndStartStationCalibrationCommand(object parameter)
        {
            _viewStarter.StartInUserContext("Warning!", "Are you sure you want to start this process?", SaveAndStartStationCalibrationCommandAction);
        }
        private void OnSaveStationCalibrationCommand(object parameter)
        {
            _viewStarter.StartInUserContext("Warning!", "Are you sure you want to start this process?", SaveStationCalibrationCommandAction);
        }

        private void OnSavedStationsHandle(Events.OnSavedStations data)
        {
            if (data != null)
            {
                if (CurrentAreas != null)
                {
                    string Areas = "";
                    int idx = 0;
                    var contours = new string[CurrentAreas.Count];
                    foreach (AreaModel areaModel in CurrentAreas)
                    {
                        Areas += areaModel.Name+";";
                        if ((areaModel.Location != null) && (areaModel.Location.Length > 0))
                        {
                            for (int i = 0; i < areaModel.Location.Length; i++)
                            {
                                contours[idx] += $"{areaModel.Location[i].Longitude} {areaModel.Location[i].Latitude}" + Environment.NewLine;
                            }
                        }
                        idx++;
                    }

                    if (data.ContextStationIds != null)
                    {
                        var modifierEditParamsCalculation = new EditParamsCalculation
                        {
                            Standard = GetStationsParams.Standard,
                            ClientContextId = data.ClientContextId,
                            TaskId = TaskId,
                            AltitudeStation = this._currentParamsCalculationModel.AltitudeStation,
                            AzimuthStation = this._currentParamsCalculationModel.AzimuthStation,
                            CascadeTuning = this._currentParamsCalculationModel.CascadeTuning,
                            CoordinatesStation = this._currentParamsCalculationModel.CoordinatesStation,
                            CorrelationDistance_m = this._currentParamsCalculationModel.CorrelationDistance_m,
                            CorrelationThresholdHard = this._currentParamsCalculationModel.CorrelationThresholdHard,
                            CorrelationThresholdWeak = this._currentParamsCalculationModel.CorrelationThresholdWeak,
                            Delta_dB = this._currentParamsCalculationModel.Delta_dB,
                            Detail = this._currentParamsCalculationModel.Detail,
                            DetailOfCascade = this._currentParamsCalculationModel.DetailOfCascade,
                            DistanceAroundContour_km = this._currentParamsCalculationModel.DistanceAroundContour_km,
                            InfocMeasResults = this._currentParamsCalculationModel.InfocMeasResults,
                            MaxAntennasPatternLoss_dB = this._currentParamsCalculationModel.MaxAntennasPatternLoss_dB,
                            MaxDeviationAltitudeStation_m = this._currentParamsCalculationModel.MaxDeviationAltitudeStation_m,
                            MaxDeviationAzimuthStation_deg = this._currentParamsCalculationModel.MaxDeviationAzimuthStation_deg,
                            MaxDeviationCoordinatesStation_m = this._currentParamsCalculationModel.MaxDeviationCoordinatesStation_m,
                            MaxDeviationTiltStation_deg = this._currentParamsCalculationModel.MaxDeviationTiltStation_deg,
                            MaxRangeMeasurements_dBmkV = this._currentParamsCalculationModel.MaxRangeMeasurements_dBmkV,
                            Method = this._currentParamsCalculationModel.Method,
                            MinNumberPointForCorrelation = this._currentParamsCalculationModel.MinNumberPointForCorrelation,
                            MinRangeMeasurements_dBmkV = this._currentParamsCalculationModel.MinRangeMeasurements_dBmkV,
                            NumberCascade = this._currentParamsCalculationModel.NumberCascade,
                            PowerStation = this._currentParamsCalculationModel.PowerStation,
                            ShiftAltitudeStationMax_m = this._currentParamsCalculationModel.ShiftAltitudeStationMax_m,
                            ShiftAltitudeStationMin_m = this._currentParamsCalculationModel.ShiftAltitudeStationMin_m,
                            ShiftAltitudeStationStep_m = this._currentParamsCalculationModel.ShiftAltitudeStationStep_m,
                            ShiftAzimuthStationMax_deg = this._currentParamsCalculationModel.ShiftAzimuthStationMax_deg,
                            ShiftAzimuthStationMin_deg = this._currentParamsCalculationModel.ShiftAzimuthStationMin_deg,
                            ShiftAzimuthStationStep_deg = this._currentParamsCalculationModel.ShiftAzimuthStationStep_deg,
                            ShiftCoordinatesStationStep_m = this._currentParamsCalculationModel.ShiftCoordinatesStationStep_m,
                            ShiftCoordinatesStation_m = this._currentParamsCalculationModel.ShiftCoordinatesStation_m,
                            ShiftPowerStationMax_dB = this._currentParamsCalculationModel.ShiftPowerStationMax_dB,
                            ShiftPowerStationMin_dB = this._currentParamsCalculationModel.ShiftPowerStationMin_dB,
                            ShiftPowerStationStep_dB = this._currentParamsCalculationModel.ShiftPowerStationStep_dB,
                            ShiftTiltStationMax_deg = this._currentParamsCalculationModel.ShiftTiltStationMax_deg,
                            ShiftTiltStationMin_deg = this._currentParamsCalculationModel.ShiftTiltStationMin_deg,
                            ShiftTiltStationStep_deg = this._currentParamsCalculationModel.ShiftTiltStationStep_deg,
                            StationIds = data.ContextStationIds,
                            Contours = contours,
                            Areas = Areas,
                            TiltStation = this._currentParamsCalculationModel.TiltStation,
                            TrustOldResults = this._currentParamsCalculationModel.TrustOldResults,
                            UseMeasurementSameGSID = this._currentParamsCalculationModel.UseMeasurementSameGSID,
                        };

                        _commandDispatcher.Send(modifierEditParamsCalculation);
                    }
                }
            }
        }

        private bool ValidateTaskParameters()
        {
            bool isSuccess = true;
            if (this.CurrentParamsCalculation != null)
            {
                // General parameter

                if (((this.CurrentParamsCalculation.CorrelationThresholdHard >= 0) && (this.CurrentParamsCalculation.CorrelationThresholdHard <= 100)) == false)
                {
                    isSuccess = false;
                
                 _viewStarter.ShowException("Error!", new Exception($"General parameter '{Resources.CorrelationThresholdHard}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.CorrelationThresholdWeak >= 0) && (this.CurrentParamsCalculation.CorrelationThresholdWeak <= 100)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"General parameter '{Resources.CorrelationThresholdWeak}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.DistanceAroundContour_km >= 0) && (this.CurrentParamsCalculation.DistanceAroundContour_km <= 50)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"General parameter '{Resources.DistanceAroundContour}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.MinNumberPointForCorrelation >= 1) && (this.CurrentParamsCalculation.MinNumberPointForCorrelation <= 1000)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"General parameter '{Resources.MinNumberPointForCorrelation}' incorrect"));
                }
                if ((this.CurrentParamsCalculation.CorrelationThresholdHard < this.CurrentParamsCalculation.CorrelationThresholdWeak))
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"Correlation threshold weak must be greater than Correlation threshold hard!"));
                }


                // Correlation parameter
                if (((this.CurrentParamsCalculation.MinRangeMeasurements_dBmkV >= -200) && (this.CurrentParamsCalculation.MinRangeMeasurements_dBmkV <= 200)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"Correlation  parameter '{Resources.MinRangeMeasurements}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.MaxRangeMeasurements_dBmkV >= -200) && (this.CurrentParamsCalculation.MaxRangeMeasurements_dBmkV <= 200)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"Correlation  parameter '{Resources.MaxRangeMeasurements}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.CorrelationDistance_m >= 0) && (this.CurrentParamsCalculation.CorrelationDistance_m <= 100)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"Correlation  parameter '{Resources.CorrelationDistance}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.Delta_dB >= 0) && (this.CurrentParamsCalculation.Delta_dB <= 20)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"Correlation  parameter '{Resources.Delta}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.MaxAntennasPatternLoss_dB >= 0) && (this.CurrentParamsCalculation.MaxAntennasPatternLoss_dB <= 200)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"Correlation  parameter '{Resources.MaxAntennasPatternLoss}' incorrect"));
                }

                // Calibration parameter
                if (((this.CurrentParamsCalculation.ShiftAltitudeStationMin_m >= -100) && (this.CurrentParamsCalculation.ShiftAltitudeStationMin_m <= 100)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.ShiftAltitudeStationMin}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.ShiftAltitudeStationMax_m >= -100) && (this.CurrentParamsCalculation.ShiftAltitudeStationMax_m <= 100)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.ShiftAltitudeStationMax}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.ShiftAltitudeStationStep_m >= 1) && (this.CurrentParamsCalculation.ShiftAltitudeStationStep_m <= 10)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.ShiftAltitudeStationStep}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.MaxDeviationAltitudeStation_m >= 0) && (this.CurrentParamsCalculation.MaxDeviationAltitudeStation_m <= 100)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.MaxDeviationAltitudeStation}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.ShiftTiltStationMin_deg >= -100) && (this.CurrentParamsCalculation.ShiftTiltStationMin_deg <= 100)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.ShiftTiltStationMin}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.ShiftTiltStationMax_deg >= -100) && (this.CurrentParamsCalculation.ShiftTiltStationMax_deg <= 100)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.ShiftTiltStationMax}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.ShiftTiltStationStep_deg >= 1) && (this.CurrentParamsCalculation.ShiftTiltStationStep_deg <= 10)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.ShiftTiltStationStep}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.MaxDeviationTiltStation_deg >= 0) && (this.CurrentParamsCalculation.MaxDeviationTiltStation_deg <= 100)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.MaxDeviationTiltStation}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.ShiftAzimuthStationMin_deg >= -200) && (this.CurrentParamsCalculation.ShiftAzimuthStationMin_deg <= 200)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.ShiftAzimuthStationMin}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.ShiftAzimuthStationMax_deg >= -200) && (this.CurrentParamsCalculation.ShiftAzimuthStationMax_deg <= 200)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.ShiftAzimuthStationMax}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.ShiftAzimuthStationStep_deg >= 1) && (this.CurrentParamsCalculation.ShiftAzimuthStationStep_deg <= 10)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.ShiftAzimuthStationStep}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.MaxDeviationAzimuthStation_deg >= 0) && (this.CurrentParamsCalculation.MaxDeviationAzimuthStation_deg <= 200)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.MaxDeviationAzimuthStation}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.ShiftCoordinatesStation_m >= 0) && (this.CurrentParamsCalculation.ShiftCoordinatesStation_m <= 1000)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.ShiftCoordinatesStation}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.ShiftCoordinatesStationStep_m >= 1) && (this.CurrentParamsCalculation.ShiftCoordinatesStationStep_m <= 100)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.ShiftCoordinatesStationStep}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.MaxDeviationCoordinatesStation_m >= 0) && (this.CurrentParamsCalculation.MaxDeviationCoordinatesStation_m <= 1000)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.MaxDeviationCoordinatesStation}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.ShiftPowerStationMin_dB >= -50) && (this.CurrentParamsCalculation.ShiftPowerStationMin_dB <= 50)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.ShiftPowerStationMin}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.ShiftPowerStationMax_dB >= -50) && (this.CurrentParamsCalculation.ShiftPowerStationMax_dB <= 50)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.ShiftPowerStationMax}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.ShiftPowerStationStep_dB >= 0) && (this.CurrentParamsCalculation.ShiftPowerStationStep_dB <= 5)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.ShiftPowerStationStep}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.NumberCascade >= 1) && (this.CurrentParamsCalculation.NumberCascade <= 5)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.NumberCascade}' incorrect"));
                }
                if (((this.CurrentParamsCalculation.DetailOfCascade >= 2) && (this.CurrentParamsCalculation.DetailOfCascade <= 10)) == false)
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"CalibrationParameters  parameter '{Resources.DetailOfCascade}' incorrect"));
                }
                if (((CurrentAreas != null) && (CurrentAreas.Count == 0)) || (CurrentAreas == null))
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception("Please fill parameter 'area!'"));

                }
                if ((CurrentAreas != null) && (CurrentAreas.Count > 0))
                {
                    foreach (AreaModel areaModel in CurrentAreas)
                    {
                        if (((areaModel.Location != null) && (areaModel.Location.Length == 0)) || (areaModel.Location == null))
                        {
                            isSuccess = false;
                            _viewStarter.ShowException("Error!", new Exception("Please fill parameter 'area!'"));
                            break;
                        }
                    }
                }


                if ((CurrentParamsCalculation.DistanceAroundContour_km == null) && ((CurrentParamsCalculation.DistanceAroundContour_km != null) && (CurrentParamsCalculation.DistanceAroundContour_km == 0)))
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"Please fill parameter 'DistanceAroundContour_km!"));
                }
                if ((string.IsNullOrEmpty(GetStationsParams.StateForActiveStation)) && (SelectedStationTypeVal== SelectedStationType.MultyStations))
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception("Please fill parameter 'StateForActiveStation!"));
                }
                if ((string.IsNullOrEmpty(GetStationsParams.StateForNotActiveStation))  && (SelectedStationTypeVal == SelectedStationType.MultyStations))
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception("Please fill parameter 'StateForNotActiveStation!"));
                }
                if ((GetStationsParams.Id == null) && (SelectedStationTypeVal == SelectedStationType.OneStation))
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception("Please fill parameter 'Id!"));
                }
                if ((string.IsNullOrEmpty(GetStationsParams.Standard))  && (SelectedStationTypeVal == SelectedStationType.MultyStations))
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception("Please fill parameter 'Standard!"));
                }
                if ((CurrentStationMonitoringModel == null) || ((CurrentStationMonitoringModel != null) && (CurrentStationMonitoringModel.Count == 0)))
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception("Please fill parameter 'Drive tests!"));
                }
                
            }
            return isSuccess;
        }
        private void SaveStationCalibrationCommandAction()
        {
            SaveStationCalibrationAction(false);
        }
        private void SaveAndStartStationCalibrationCommandAction()
        {
            SaveStationCalibrationAction(true);
        }
        private void SaveStationCalibrationAction(bool isStart = false)
        {
            //bool isSuccess = false;
            if (ValidateTaskParameters())
            {
                var listStationMonitoringModel = new List<long>();
                foreach (StationMonitoringModel x in CurrentStationMonitoringModel)
                {
                    var driveTestStandardStats = x.DriveTestStandardStats;
                    if (driveTestStandardStats != null)
                    {
                        var listDriveTests = driveTestStandardStats.ToList();
                        for (int k = 0; k < listDriveTests.Count; k++)
                        {
                            var max = GetMaximumCountPointsInDriveTests(listDriveTests[k].Standard);
                            if (listDriveTests[k].Count > max)
                            {
                                _viewStarter.ShowException("Error!", new Exception($"The functionality cannot be started, because for the standard '{listDriveTests[k].Standard}' the  number of points greater  {listDriveTests[k].Count}!"));
                                return;
                            }
                            if (listDriveTests[k].Count == 0)
                            {
                                _viewStarter.ShowException("Error!", new Exception($"The functionality cannot be started, because for the standard '{listDriveTests[k].Standard}' the  number of points is 0!"));
                                return;
                            }
                        }
                    }
                    listStationMonitoringModel.Add(x.Id);
                }

                Task.Run(() =>
                {
                    var isSuccess = OnLoadStationsHandle(listStationMonitoringModel);
                    if (isStart && isSuccess)
                    {
                        var modifier = new MG.Modifiers.RunCalcTask { Id = TaskId };
                        _commandDispatcher.Send(modifier);
                    }
                });

                //_viewStarter.StartLongProcess(
                //    new LongProcessOptions()
                //    {
                //        CanStop = false,
                //        CanAbort = false,
                //        UseProgressBar = true,
                //        UseLog = false,
                //        IsModal = true,
                //        MinValue = 0,
                //        MaxValue = 1000,
                //        ValueKind = LongProcessValueKind.Infinity,
                //        Title = "Saving stations ...",
                //        Note = "Selection and saving of stations in the calculation server in accordance with the specified parameters."
                //    },
                //    token =>
                //    {
                //        isSuccess = OnLoadStationsHandle(listStationMonitoringModel);
                //    });
                //if (isSuccess)
                //{
                _viewStarter.Stop(this);
                //}
            }
        }
        private void OnEditParamsCalculationsHandle(Events.OnEditParamsCalculation data)
        {
            if (data != null)
            {
                if (data.IsSuccessUpdateParameters)
                {
                    var modifierUpdateClientContext = new UpdateClientContext
                    {
                        ClientContextId = data.ClientContextId
                    };
                    _commandDispatcher.Send(modifierUpdateClientContext);
                }
            }
        }
        private void OnCreatePropagationModelsHandle(Events.OnCreatePropagationModels data)
        {
            if (data != null)
            {

            }
        }

        private bool OnLoadStationsHandle(List<long> listStationMonitoringModel)
        {
            bool isSuccess = true;
            try
            {
                var stations = ReadStations(out string standard);
                if (stations.Length > 0)
                {
                    //если выбран режим "OneStation" то сохраняем реальный стандарт станции
                    if (SelectedStationTypeVal == SelectedStationType.OneStation)
                    {
                        GetStationsParams.Standard = standard;
                    }
                    this._currentParamsCalculationModel.InfocMeasResults = listStationMonitoringModel.ToArray();
                    this._currentParamsCalculationModel.StationIds = stations.Select(x => Convert.ToInt64(x.ExternalCode)).ToArray();
                    var clientContextId = _objectReader.Read<long?>().By(new CalcTaskModelByContextId() { TaskId = TaskId });
                    if ((clientContextId != null) && (clientContextId != 0))
                    {
                        var createPropagationModels = new CreatePropagationModels()
                        {
                            ContextId = clientContextId.Value
                        };
                        _commandDispatcher.Send(createPropagationModels);

                        var createClientContextStations = new CreateClientContextStations()
                        {
                            ClientContextId = clientContextId.Value,
                            IcsmMobStation = stations
                        };
                        _commandDispatcher.Send(createClientContextStations);
                    }
                    else
                    {
                        isSuccess = false;
                        _viewStarter.ShowException("Error!", new Exception($"Client context Id is 0!"));
                    }
                }
                else
                {
                    isSuccess = false;
                    _viewStarter.ShowException("Error!", new Exception($"No stations with suitable parameters!"));
                }
            }
            catch (Exception e)
            {
                isSuccess = false;
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
            return isSuccess;
        }
    }
    public enum TypeCoord
    {
        RecalcLatitude,
        RecalcLongitude,
    }

}
