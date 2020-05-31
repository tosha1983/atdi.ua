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
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.Contracts.Sdrn.DeepServices.Gis;




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

        private readonly ViewStarter _viewStarter;
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
        private readonly ITransformation _transformation;


        private ParamsCalculationModel _currentParamsCalculationModel;
        private IList _currentStationMonitoringModel;
        private GetStationsParamsModel _currentGetStationsParameters;
       


        private AreasDataAdapter AreasDataAdapter;
        public StationMonitoringDataAdapter StationMonitoringDataAdapter { get; set; }
        public ParametersDataAdapter ParametersDataAdapter { get; set; }

        public ViewCommand StartStationCalibrationCommand { get; set; }
        public ViewCommand LoadDriveTestsCommand { get; set; }


        private CalcServerDataLayer _dataLayer { get; set; }


        public View(
            ViewStarter viewStarter,
            CalcServerDataLayer dataLayer,
            ParametersDataAdapter parametersDataAdapter,
            StationMonitoringDataAdapter stationMonitoringDataAdapter,
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


            this.StartStationCalibrationCommand = new ViewCommand(this.OnStartStationCalibrationCommand);
            this.LoadDriveTestsCommand = new ViewCommand(this.OnLoadDriveTestsCommand);

            this._currentParamsCalculationModel = new ParamsCalculationModel();
            this._currentGetStationsParameters = new GetStationsParamsModel();

            this.AreasDataAdapter = new AreasDataAdapter();
            this.ParametersDataAdapter = parametersDataAdapter;
            this.StationMonitoringDataAdapter = stationMonitoringDataAdapter;
            this._transformation = transformation;


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
            if ((this._currentParamsCalculationModel.CorrelationThresholdHard==null) && (this._currentParamsCalculationModel.CorrelationThresholdWeak == null) || (this._currentParamsCalculationModel.DistanceAroundContour_km == null))
            {
                FillParametersDefault(ref this._currentParamsCalculationModel);
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

        private void FillParametersDefault(ref ParamsCalculationModel paramsCalculationModel)
        {
            paramsCalculationModel.CorrelationThresholdWeak = 85;
            paramsCalculationModel.CorrelationThresholdHard = 30;
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
            paramsCalculationModel.ShiftTiltStationMin_deg = -20;
            paramsCalculationModel.ShiftTiltStationMax_deg = 0;
            paramsCalculationModel.ShiftTiltStationStep_deg = 1;
            paramsCalculationModel.MaxDeviationTiltStation_deg = 3;
            paramsCalculationModel.AzimuthStation = true;
            paramsCalculationModel.ShiftAltitudeStationMin_m = -180;
            paramsCalculationModel.ShiftAltitudeStationMax_m = 180;
            paramsCalculationModel.ShiftAltitudeStationStep_m = 10;
            paramsCalculationModel.ShiftAzimuthStationMax_deg = 30;
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
            get => this._selectedStationType;
            set => this.Set(ref this._selectedStationType, value);
        }

        public MethodParamsCalculationModel MethodParamsCalculationModelVal
        {
            get {return  this._methodParamsCalculationModel; }
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
                siteEPSG.Y = siteEPSG.Y + distance_km * 1000;
                wgs84Coordinate = this._transformation.ConvertCoordinateToWgs84(siteEPSG, 31466);
            }
            if (typeCoord == TypeCoord.RecalcLongitude)
            {
                siteEPSG.X = siteEPSG.X + distance_km * 1000;
                wgs84Coordinate = this._transformation.ConvertCoordinateToWgs84(siteEPSG, 31466);
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

                if (this._currentAreas != null)
                    foreach (AreaModel area in this._currentAreas)
                    {
                        if (area.Location != null)
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

        private void OnStartStationCalibrationCommand(object parameter)
        {

            try
            {
                if (((CurrentAreas != null) && (CurrentAreas.Count == 0)) || (CurrentAreas == null))
                {
                    System.Windows.Forms.MessageBox.Show("Please select area!");
                    return;
                }
                if ((CurrentParamsCalculation.DistanceAroundContour_km == null) || ((CurrentParamsCalculation.DistanceAroundContour_km != null) && (CurrentParamsCalculation.DistanceAroundContour_km == 0)))
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
                if ((GetStationsParams.Id == null) && (SelectedStationTypeVal == SelectedStationType.OneStation))
                {
                    System.Windows.Forms.MessageBox.Show("Please fill 'Id'");
                    return;
                }
                if (string.IsNullOrEmpty(GetStationsParams.Standard))
                {
                    System.Windows.Forms.MessageBox.Show("Please fill 'Standard'");
                    return;
                }
                if ((CurrentStationMonitoringModel == null) || ((CurrentStationMonitoringModel != null) && (CurrentStationMonitoringModel.Count == 0)))
                {
                    System.Windows.Forms.MessageBox.Show("Please select 'Drive tests'");
                    return;
                }


                var stations = ReadStations();
                if (stations.Length > 0)
                {
                    var listStationMonitoringModel = new List<long>();
                    foreach (StationMonitoringModel x in CurrentStationMonitoringModel)
                    {
                        listStationMonitoringModel.Add(x.Id);
                    }

                    this._currentParamsCalculationModel.InfocMeasResults = listStationMonitoringModel.ToArray();
                    this._currentParamsCalculationModel.StationIds = stations.Select(x => Convert.ToInt64(x.ExternalCode)).ToArray();
                    StationCalibrationCalcTask.SaveTask(this._dataLayer.Origin, this._dataLayer.Executor, stations, this._currentParamsCalculationModel, TaskId);
                    //StationCalibrationCalcTask.RunTask(this._dataLayer.Origin, this._dataLayer.Executor, TaskId);

                    System.Windows.MessageBox.Show($"Task saved with {this._currentParamsCalculationModel.StationIds.Length} stations and {CurrentStationMonitoringModel.Count} drive tests");
                }
                else
                {
                    System.Windows.MessageBox.Show("No stations with suitable parameters!");
                }
                _viewStarter.Stop(this);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }

    }
    public enum TypeCoord
    {
        RecalcLatitude,
        RecalcLongitude,
    }

}
