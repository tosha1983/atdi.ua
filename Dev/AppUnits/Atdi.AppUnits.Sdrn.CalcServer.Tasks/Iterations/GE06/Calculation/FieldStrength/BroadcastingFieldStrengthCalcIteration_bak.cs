﻿//using Atdi.Contracts.Sdrn.CalcServer;
//using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
//using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
//using Atdi.DataModels.Sdrn.DeepServices.Gis;
//using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
//using Atdi.Platform.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Atdi.Contracts.Sdrn.CalcServer.Internal;
//using Atdi.Contracts.Sdrn.DeepServices.Gis;
//using Atdi.DataModels.Sdrn.DeepServices.Gis.MapService;
//using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;
//using Atdi.DataModels.Sdrn.DeepServices.GN06;
//using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
//using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
//using System.Runtime.InteropServices.WindowsRuntime;
//using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;
//using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.FieldStrength;
//using Atdi.DataModels.Sdrn.CalcServer;
//using Atdi.DataModels.Sdrn.CalcServer.Entities;

//namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
//{
//    /// <summary>
//    /// Вариация BroadcastingFieldStrengthCalcIteration с правильным расчётом геометрии
//    /// но 
//    ///  разницы в расчётах не обнаружено
//    /// считает в 2-3 раза медленней
//    /// </summary>
//    public class BroadcastingFieldStrengthCalcIteration_bak : IIterationHandler<BroadcastingFieldStrengthCalcData, BroadcastingFieldStrengthCalcResult>
//    {
//        private readonly ISignalService _signalService;
//        private readonly IMapService _mapService;
//        private readonly IObjectPoolSite _poolSite;
//        private readonly IObjectPool<ProfileIndexer[]> _indexerArrayPool;
//        private readonly IObjectPool<byte[]> _clutterArrayPool;
//        private readonly IObjectPool<byte[]> _buildingArrayPool;
//        private readonly IObjectPool<short[]> _reliefArrayPool;
//        private readonly IObjectPool<short[]> _heightArrayPool;
//        private readonly IEarthGeometricService _earthGeometricService;
//        private readonly ITransformation _transformation;



//        #region Profile Config 
//        private static readonly Dictionary<MainCalcBlockModelType, ProfileOptions> MainBlockProfileOptions = new Dictionary<MainCalcBlockModelType, ProfileOptions>
//        {
//            [MainCalcBlockModelType.Unknown] = ProfileOptions.Empty,
//            [MainCalcBlockModelType.ITU2001] = ProfileOptions.OnlyHeight,
//            [MainCalcBlockModelType.ITU1546] = ProfileOptions.ReliefAndClutter,
//            [MainCalcBlockModelType.ITU525] = ProfileOptions.Empty
//        };

//        private static readonly Dictionary<AbsorptionCalcBlockModelType, ProfileOptions> AbsorptionBlockProfileOptions = new Dictionary<AbsorptionCalcBlockModelType, ProfileOptions>
//        {
//            [AbsorptionCalcBlockModelType.Unknown] = ProfileOptions.Empty,
//            [AbsorptionCalcBlockModelType.Flat] = ProfileOptions.AllWithoutHeight,
//            [AbsorptionCalcBlockModelType.FlatAndLinear] = ProfileOptions.AllWithoutHeight,
//            [AbsorptionCalcBlockModelType.ITU2109AndLinear] = ProfileOptions.AllWithoutHeight,
//            [AbsorptionCalcBlockModelType.ITU2109_2] = ProfileOptions.AllWithoutHeight,
//            [AbsorptionCalcBlockModelType.Linear] = ProfileOptions.AllWithoutHeight,

//        };

//        private static readonly Dictionary<AdditionalCalcBlockModelType, ProfileOptions> AdditionalBlockProfileOptions = new Dictionary<AdditionalCalcBlockModelType, ProfileOptions>
//        {
//            [AdditionalCalcBlockModelType.Unknown] = ProfileOptions.Empty,
//            [AdditionalCalcBlockModelType.ITU1820] = ProfileOptions.Empty

//        };

//        private static readonly Dictionary<AtmosphericCalcBlockModelType, ProfileOptions> AtmosphericBlockProfileOptions = new Dictionary<AtmosphericCalcBlockModelType, ProfileOptions>
//        {
//            [AtmosphericCalcBlockModelType.Unknown] = ProfileOptions.Empty,
//            [AtmosphericCalcBlockModelType.ITU1820] = ProfileOptions.Empty,
//            [AtmosphericCalcBlockModelType.ITU838_530] = ProfileOptions.OnlyHeight,
//            [AtmosphericCalcBlockModelType.ITU678] = ProfileOptions.Empty,
//        };

//        private static readonly Dictionary<ClutterCalcBlockModelType, ProfileOptions> ClutterBlockProfileOptions = new Dictionary<ClutterCalcBlockModelType, ProfileOptions>
//        {
//            [ClutterCalcBlockModelType.Unknown] = ProfileOptions.Empty,
//            [ClutterCalcBlockModelType.ITU2109] = ProfileOptions.AllWithoutHeight,
//            [ClutterCalcBlockModelType.Flat] = ProfileOptions.AllWithoutHeight
//        };

//        private static readonly Dictionary<DiffractionCalcBlockModelType, ProfileOptions> DiffractionBlockProfileOptions = new Dictionary<DiffractionCalcBlockModelType, ProfileOptions>
//        {
//            [DiffractionCalcBlockModelType.Unknown] = ProfileOptions.Empty,
//            [DiffractionCalcBlockModelType.Bullington] = ProfileOptions.OnlyHeight,
//            [DiffractionCalcBlockModelType.Deygout66] = ProfileOptions.OnlyHeight,
//            [DiffractionCalcBlockModelType.Deygout91] = ProfileOptions.OnlyHeight,
//            [DiffractionCalcBlockModelType.ITU526_15] = ProfileOptions.OnlyHeight,

//        };

//        private static readonly Dictionary<SubPathDiffractionCalcBlockModelType, ProfileOptions> SubPathDiffractionBlockProfileOptions = new Dictionary<SubPathDiffractionCalcBlockModelType, ProfileOptions>
//        {
//            [SubPathDiffractionCalcBlockModelType.Unknown] = ProfileOptions.Empty,
//            [SubPathDiffractionCalcBlockModelType.SubDeygout91] = ProfileOptions.Empty
//        };

//        private static readonly Dictionary<TropoCalcBlockModelType, ProfileOptions> TropoBlockProfileOptions = new Dictionary<TropoCalcBlockModelType, ProfileOptions>
//        {
//            [TropoCalcBlockModelType.Unknown] = ProfileOptions.Empty,
//            [TropoCalcBlockModelType.ITU617] = ProfileOptions.OnlyHeight
//        };

//        private static readonly ProfileOptions DuctingBlockProfileOptions = ProfileOptions.OnlyHeight;

//        private static readonly ProfileOptions ReflectionBlockProfileOptions = ProfileOptions.Empty;

//        private struct ProfileOptions
//        {
//            public static ProfileOptions Empty = new ProfileOptions();

//            public static ProfileOptions AllWithoutHeight = new ProfileOptions
//            {
//                Relief = true,
//                Clutter = true,
//                Building = true,
//                Height = false
//            };

//            public static ProfileOptions OnlyHeight = new ProfileOptions
//            {
//                Relief = false,
//                Clutter = false,
//                Building = false,
//                Height = true
//            };

//            public static ProfileOptions ReliefAndClutter = new ProfileOptions
//            {
//                Relief = true,
//                Clutter = true,
//                Building = false,
//                Height = false
//            };

//            public bool Clutter;
//            public bool Relief;
//            public bool Building;
//            public bool Height;

//            public bool IsNotEmpty => Relief || Clutter || Building || Height;

//            public void Apply(ProfileOptions source)
//            {
//                if (source.Building)
//                {
//                    this.Building = true;
//                }
//                if (source.Clutter)
//                {
//                    this.Clutter = true;
//                }
//                if (source.Height)
//                {
//                    this.Height = true;
//                }
//                if (source.Relief)
//                {
//                    this.Relief = true;
//                }
//            }
//        }

//        #endregion

//        private static double LinearInterpolationFuncOfAngle(double angle, float angle0, float angle1, float angleValue0, float angleValue1)
//        {
//            double value = angleValue0 + (angleValue1 - angleValue0) / (angle1 - angle0) * (angle - angle0);
//            return value;
//        }

//        public BroadcastingFieldStrengthCalcIteration_bak(
//            ISignalService signalService,
//            IEarthGeometricService earthGeometricService,
//            IMapService mapService,
//            IObjectPoolSite poolSite,
//            ITransformation transformation)
//        {
//            _signalService = signalService;
//            _mapService = mapService;
//            _poolSite = poolSite;
//            _earthGeometricService = earthGeometricService;

//            _indexerArrayPool = _poolSite.GetPool<ProfileIndexer[]>(ObjectPools.GisProfileIndexerArrayObjectPool);
//            _clutterArrayPool = _poolSite.GetPool<byte[]>(ObjectPools.GisProfileClutterArrayObjectPool);
//            _buildingArrayPool = _poolSite.GetPool<byte[]>(ObjectPools.GisProfileBuildingArrayObjectPool);
//            _reliefArrayPool = _poolSite.GetPool<short[]>(ObjectPools.GisProfileReliefArrayObjectPool);
//            _heightArrayPool = _poolSite.GetPool<short[]>(ObjectPools.GisProfileHeightArrayObjectPool);
//            _transformation = transformation;
//        }

//        private void CalcLandSeaOnProfile(CalcProfileIndexersArgs profileArgs, BroadcastingFieldStrengthCalcData data, ITaskContext taskContext, double path_km, ref LandSea landSea, ref bool h2aboveSea)
//        {
//            var profileOptions = DefineProfileOptions(data.PropagationModel);
//            var indexerBuffer = default(ProfileIndexer[]);
//            var clutterBuffer = default(byte[]);
//            var profileLenght = 0;

//            if (profileOptions.IsNotEmpty)
//            {
//                try
//                {
//                    //  получаем буффер под профиль
//                    indexerBuffer = _indexerArrayPool.Take();

//                    var lowerLeftCoord_m = data.MapArea.LowerLeft;
//                    var upperRightCoord_m = data.MapArea.UpperRight;
//                    var isInsideTargetCoordinate = Utils.IsInsideMap(profileArgs.Target.X, profileArgs.Target.Y, lowerLeftCoord_m.X, lowerLeftCoord_m.Y, upperRightCoord_m.X, upperRightCoord_m.Y);
//                    var isInsidePointCoordinate = Utils.IsInsideMap(profileArgs.Point.X, profileArgs.Point.Y, lowerLeftCoord_m.X, lowerLeftCoord_m.Y, upperRightCoord_m.X, upperRightCoord_m.Y);

//                    if ((isInsideTargetCoordinate == false) || (isInsidePointCoordinate == false))
//                    {
//                        if (isInsideTargetCoordinate == false)
//                        {
//                            string message = $"Starting the iteration 'IIterationHandler <BroadcastingFieldStrengthCalcData, BroadcastingFieldStrengthCalcResult>' could not be started because the coordinates of the point  'Target.X={profileArgs.Target.X}','Target.Y={profileArgs.Target.Y}' not included in map range!";
//                            taskContext.SendEvent(new CalcResultEvent
//                            {
//                                Level = CalcResultEventLevel.Error,
//                                Context = "Ge06CalcIteration",
//                                Message = message
//                            });
//                            throw new Exception(message);
//                        }
//                        if (isInsidePointCoordinate == false)
//                        {
//                            string message = $"Starting the iteration 'IIterationHandler <BroadcastingFieldStrengthCalcData, BroadcastingFieldStrengthCalcResult>' could not be started because the coordinates of the point  'Point.X={profileArgs.Point.X}','Point.Y={profileArgs.Point.Y}' not included in map range!";
//                            taskContext.SendEvent(new CalcResultEvent
//                            {
//                                Level = CalcResultEventLevel.Error,
//                                Context = "Ge06CalcIteration",
//                                Message = message
//                            });
//                            throw new Exception(message);
//                        }
//                    }

//                    var profileResult = new CalcProfileIndexersResult
//                    {
//                        Indexers = indexerBuffer,
//                        StartPosition = 0,
//                        IndexerCount = 0
//                    };
//                    _mapService.CalcProfileIndexers(in profileArgs, ref profileResult);
//                    profileLenght = profileResult.IndexerCount;

//                    // получаем  буферы массивов


//                    if (profileOptions.Clutter)
//                    {
//                        clutterBuffer = _clutterArrayPool.Take();
//                    }

//                    var axisXNumber = data.MapArea.AxisX.Number;
//                    for (int i = 0; i < profileResult.IndexerCount; i++)
//                    {
//                        var indexer = indexerBuffer[i];
//                        var contentIndex = indexer.YIndex * axisXNumber + indexer.XIndex;


//                        var clutterValue = MapSpecification.DefaultForClutter;
//                        //int clutterH = 0;
//                        if (profileOptions.Clutter)
//                        {
//                            clutterValue = data.ClutterContent[contentIndex];
//                            clutterBuffer[i] = clutterValue;
//                        }
//                    }

//                    // расчет длинны по воде и длинны по морю

//                    double px2km = path_km / (profileLenght - 1);
//                    byte waterClutter = 6; // clutter code for sea/inland water should be in config
//                    int aboveLand_px = 0;
//                    int aboveSea_px = 0;


//                    for (int i = 0; i < profileLenght; i++)
//                    {
//                        // count relief points that belongs to land or sea propagation
//                        if (clutterBuffer[i] == waterClutter)
//                        {
//                            aboveSea_px++;
//                            if (i == profileLenght - 1)
//                            {
//                                h2aboveSea = true;
//                            }
//                        }
//                        else
//                        {
//                            aboveLand_px++;
//                        }
//                    }

//                    landSea = new LandSea { land = aboveLand_px * px2km, sea = aboveSea_px * px2km };

//                }
//                catch (Exception)
//                {
//                    throw;
//                }
//                finally
//                {
//                    if (indexerBuffer != null)
//                    {
//                        _indexerArrayPool.Put(indexerBuffer);
//                    }
//                    if (clutterBuffer != null)
//                    {
//                        _clutterArrayPool.Put(clutterBuffer);
//                    }

//                }
//            }
//        }
//        private void CalcHeff(CalcProfileIndexersArgs profileArgs, BroadcastingFieldStrengthCalcData data, ITaskContext taskContext, ref double effectiveHeight)
//        {
//            var profileOptions = DefineProfileOptions(data.PropagationModel);
//            var indexerBuffer = default(ProfileIndexer[]);
//            var reliefBuffer = default(short[]);
//            var profileLenght = 0;

//            if (profileOptions.IsNotEmpty)
//            {
//                try
//                {
//                    //  получаем буффер под профиль
//                    indexerBuffer = _indexerArrayPool.Take();

//                    var lowerLeftCoord_m = data.MapArea.LowerLeft;
//                    var upperRightCoord_m = data.MapArea.UpperRight;
//                    var isInsideTargetCoordinate = Utils.IsInsideMap(profileArgs.Target.X, profileArgs.Target.Y, lowerLeftCoord_m.X, lowerLeftCoord_m.Y, upperRightCoord_m.X, upperRightCoord_m.Y);
//                    var isInsidePointCoordinate = Utils.IsInsideMap(profileArgs.Point.X, profileArgs.Point.Y, lowerLeftCoord_m.X, lowerLeftCoord_m.Y, upperRightCoord_m.X, upperRightCoord_m.Y);

//                    if ((isInsideTargetCoordinate == false) || (isInsidePointCoordinate == false))
//                    {
//                        if (isInsideTargetCoordinate == false)
//                        {
//                            string message = $"Starting the iteration 'IIterationHandler <BroadcastingFieldStrengthCalcData, BroadcastingFieldStrengthCalcResult>' could not be started because the coordinates of the point  'Target.X={profileArgs.Target.X}','Target.Y={profileArgs.Target.Y}' not included in map range!";
//                            taskContext.SendEvent(new CalcResultEvent
//                            {
//                                Level = CalcResultEventLevel.Error,
//                                Context = "Ge06CalcIteration",
//                                Message = message
//                            });
//                            throw new Exception(message);
//                        }
//                        if (isInsidePointCoordinate == false)
//                        {
//                            string message = $"Starting the iteration 'IIterationHandler <BroadcastingFieldStrengthCalcData, BroadcastingFieldStrengthCalcResult>' could not be started because the coordinates of the point  'Point.X={profileArgs.Point.X}','Point.Y={profileArgs.Point.Y}' not included in map range!";
//                            taskContext.SendEvent(new CalcResultEvent
//                            {
//                                Level = CalcResultEventLevel.Error,
//                                Context = "Ge06CalcIteration",
//                                Message = message
//                            });
//                            throw new Exception(message);
//                        }
//                    }

//                    var profileResult = new CalcProfileIndexersResult
//                    {
//                        Indexers = indexerBuffer,
//                        StartPosition = 0,
//                        IndexerCount = 0
//                    };
//                    _mapService.CalcProfileIndexers(in profileArgs, ref profileResult);
//                    profileLenght = profileResult.IndexerCount;

//                    // получаем  буферы массивов
//                    if (profileOptions.Relief)
//                    {
//                        reliefBuffer = _reliefArrayPool.Take();
//                    }

//                    var axisXNumber = data.MapArea.AxisX.Number;
//                    for (int i = 0; i < profileResult.IndexerCount; i++)
//                    {
//                        var indexer = indexerBuffer[i];
//                        var contentIndex = indexer.YIndex * axisXNumber + indexer.XIndex;

//                        var reliefValue = MapSpecification.DefaultForRelief;
//                        if (profileOptions.Relief)
//                        {
//                            reliefValue = data.ReliefContent[contentIndex];
//                            if (profileOptions.Relief)
//                            {
//                                reliefBuffer[i] = reliefValue;
//                            }
//                        }
//                    }

//                    // расчет средней высоты рельефа на растоянии 15км
//                    double px2km = 15.0 / (profileLenght - 1);
//                    int hMedDist_px = 0;
//                    // calculate effective height
//                    int minDistToHeff_px = (int)(3.0 / px2km);
//                    int maxDistToHeff_px = (int)Math.Min(15.0 / px2km, profileLenght - 1);
//                    if (minDistToHeff_px < maxDistToHeff_px)
//                    {
//                        for (int i = minDistToHeff_px; i < maxDistToHeff_px; i++)
//                        {
//                            effectiveHeight += reliefBuffer[0] - reliefBuffer[i];
//                            hMedDist_px++;

//                        }
//                        if (hMedDist_px > 0)
//                        {
//                            effectiveHeight /= hMedDist_px;
//                            effectiveHeight += data.BroadcastingAssignment.AntennaCharacteristics.AglHeight_m;
//                        }
//                    }

//                }
//                catch (Exception)
//                {
//                    throw;
//                }
//                finally
//                {
//                    if (indexerBuffer != null)
//                    {
//                        _indexerArrayPool.Put(indexerBuffer);
//                    }

//                    if (reliefBuffer != null)
//                    {
//                        _reliefArrayPool.Put(reliefBuffer);
//                    }
//                }


//            }
//        }

//        public BroadcastingFieldStrengthCalcResult Run(ITaskContext taskContext, BroadcastingFieldStrengthCalcData data)
//        {


//            try
//            {
//                // расчет точек для проведения расчета 
//                var pointSourceArgs = new PointEarthGeometric() { Longitude = data.BroadcastingAssignment.SiteParameters.Lon_Dec, Latitude = data.BroadcastingAssignment.SiteParameters.Lat_Dec, CoordinateUnits = CoordinateUnits.deg };
//                var d_km = this._earthGeometricService.GetDistance_km(in pointSourceArgs, in data.TargetCoordinate);
//                var AzimuthToTarget = this._earthGeometricService.GetAzimut(in pointSourceArgs, in data.TargetCoordinate);


//                //var profileOptions = DefineProfileOptions(data.PropagationModel);


//                int azimuth0 = (int)(Math.Floor(AzimuthToTarget / 10) * 10);
//                int azimuth1 = azimuth0 + 10;
//                if (azimuth1 == 360)
//                {
//                    azimuth1 = 0;
//                }

//                //double antennaGain_dB = 0;
//                double ERP_dBW = 0;
//                switch (data.BroadcastingAssignment.EmissionCharacteristics.Polar)
//                {
//                    case PolarType.H:
//                        if ((data.BroadcastingAssignment.AntennaCharacteristics.Direction == AntennaDirectionType.D) && (data.BroadcastingAssignment.AntennaCharacteristics.DiagrH != null))
//                        {
//                            ERP_dBW = data.BroadcastingAssignment.EmissionCharacteristics.ErpH_dBW - LinearInterpolationFuncOfAngle(AzimuthToTarget, azimuth0, azimuth0 + 10, data.BroadcastingAssignment.AntennaCharacteristics.DiagrH[azimuth0 / 10], data.BroadcastingAssignment.AntennaCharacteristics.DiagrH[azimuth1 / 10]);
//                        }
//                        else
//                        {
//                            ERP_dBW = data.BroadcastingAssignment.EmissionCharacteristics.ErpH_dBW;
//                        }
//                        break;
//                    case PolarType.V:
//                        if ((data.BroadcastingAssignment.AntennaCharacteristics.Direction == AntennaDirectionType.D) && (data.BroadcastingAssignment.AntennaCharacteristics.DiagrV != null))
//                        {
//                            ERP_dBW = data.BroadcastingAssignment.EmissionCharacteristics.ErpV_dBW - LinearInterpolationFuncOfAngle(AzimuthToTarget, azimuth0, azimuth0 + 10, data.BroadcastingAssignment.AntennaCharacteristics.DiagrV[azimuth0 / 10], data.BroadcastingAssignment.AntennaCharacteristics.DiagrV[azimuth1 / 10]);
//                        }
//                        else
//                        {
//                            ERP_dBW = data.BroadcastingAssignment.EmissionCharacteristics.ErpV_dBW;
//                        }
//                        break;
//                    case PolarType.M:
//                        if ((data.BroadcastingAssignment.AntennaCharacteristics.Direction == AntennaDirectionType.D) && (data.BroadcastingAssignment.AntennaCharacteristics.DiagrH != null && data.BroadcastingAssignment.AntennaCharacteristics.DiagrV != null))
//                        {
//                            ERP_dBW = 10 * Math.Log10(Math.Pow(10, 0.1 * (data.BroadcastingAssignment.EmissionCharacteristics.ErpH_dBW - LinearInterpolationFuncOfAngle(AzimuthToTarget, azimuth0, azimuth0 + 10, data.BroadcastingAssignment.AntennaCharacteristics.DiagrH[azimuth0 / 10], data.BroadcastingAssignment.AntennaCharacteristics.DiagrH[azimuth1 / 10]))) + Math.Pow(10, 0.1 * (data.BroadcastingAssignment.EmissionCharacteristics.ErpV_dBW - LinearInterpolationFuncOfAngle(AzimuthToTarget, azimuth0, azimuth0 + 10, data.BroadcastingAssignment.AntennaCharacteristics.DiagrV[azimuth0 / 10], data.BroadcastingAssignment.AntennaCharacteristics.DiagrV[azimuth1 / 10]))));
//                        }
//                        else
//                        {
//                            ERP_dBW = 10 * Math.Log10(Math.Pow(10, 0.1 * data.BroadcastingAssignment.EmissionCharacteristics.ErpH_dBW) + Math.Pow(10, 0.1 * data.BroadcastingAssignment.EmissionCharacteristics.ErpV_dBW));
//                        }
//                        break;
//                    default:
//                        //ERP_dBW = 10 * Math.Log10(Math.Pow(10, 0.1 * (data.BroadcastingAssignment.EmissionCharacteristics.ErpH_dBW - LinearInterpolationFuncOfAngle(AzimuthToTarget, azimuth0, azimuth0 + 10, data.BroadcastingAssignment.AntennaCharacteristics.DiagrH[azimuth0 / 10], data.BroadcastingAssignment.AntennaCharacteristics.DiagrH[azimuth1 / 10]))) + Math.Pow(10, 0.1 * (data.BroadcastingAssignment.EmissionCharacteristics.ErpV_dBW - LinearInterpolationFuncOfAngle(AzimuthToTarget, azimuth0, azimuth0 + 10, data.BroadcastingAssignment.AntennaCharacteristics.DiagrV[azimuth0 / 10], data.BroadcastingAssignment.AntennaCharacteristics.DiagrV[azimuth1 / 10]))));
//                        //if ((data.BroadcastingAssignment.AntennaCharacteristics.Direction == AntennaDirectionType.D) && (data.BroadcastingAssignment.AntennaCharacteristics.DiagrH != null && data.BroadcastingAssignment.AntennaCharacteristics.DiagrV != null))
//                        //{
//                        //    ERP_dBW = 10 * Math.Log10(Math.Pow(10, 0.1 * (data.BroadcastingAssignment.EmissionCharacteristics.ErpH_dBW - LinearInterpolationFuncOfAngle(AzimuthToTarget, azimuth0, azimuth0 + 10, data.BroadcastingAssignment.AntennaCharacteristics.DiagrH[azimuth0 / 10], data.BroadcastingAssignment.AntennaCharacteristics.DiagrH[azimuth1 / 10]))) + Math.Pow(10, 0.1 * (data.BroadcastingAssignment.EmissionCharacteristics.ErpV_dBW - LinearInterpolationFuncOfAngle(AzimuthToTarget, azimuth0, azimuth0 + 10, data.BroadcastingAssignment.AntennaCharacteristics.DiagrV[azimuth0 / 10], data.BroadcastingAssignment.AntennaCharacteristics.DiagrV[azimuth1 / 10]))));
//                        //}
//                        //else
//                        //{
//                        //    ERP_dBW = 10 * Math.Log10(Math.Pow(10, 0.1 * data.BroadcastingAssignment.EmissionCharacteristics.ErpH_dBW) + Math.Pow(10, 0.1 * data.BroadcastingAssignment.EmissionCharacteristics.ErpV_dBW));
//                        //}
//                        break;
//                }

//                //

//                // prepare data





//                //

//                // effective height estimation
//                double effectiveHeight = 0.0;
//                bool isheffGiven = false;
//                if (data.BroadcastingAssignment.AntennaCharacteristics.EffHeight_m != null)
//                {
//                    for (int i = 0; i < data.BroadcastingAssignment.AntennaCharacteristics.EffHeight_m.Length; i++)
//                    {
//                        if (data.BroadcastingAssignment.AntennaCharacteristics.EffHeight_m[i] != 0)
//                        {
//                            isheffGiven = true;
//                            break;
//                        }
//                    }
//                }




//                ////////


//                if (isheffGiven)
//                {
//                    effectiveHeight = LinearInterpolationFuncOfAngle(AzimuthToTarget, azimuth0, azimuth1, data.BroadcastingAssignment.AntennaCharacteristics.EffHeight_m[azimuth0 / 10], data.BroadcastingAssignment.AntennaCharacteristics.EffHeight_m[azimuth1 / 10]);
//                }
//                else if (data.ReliefContent != null)
//                {
//                    // определяем профиль
//                    //PointEarthGeometric startPoint = new PointEarthGeometric { Longitude = data.BroadcastingAssignment.SiteParameters.Lon_Dec, Latitude = data.BroadcastingAssignment.SiteParameters.Lat_Dec };
//                    var pointCoord = this._earthGeometricService.CalculationCoordinateByLengthAndAzimuth(pointSourceArgs, 15, AzimuthToTarget);
//                    var profileArgs = new CalcProfileIndexersArgs
//                    {
//                        AxisXStep = data.MapArea.AxisX.Step,
//                        AxisYStep = data.MapArea.AxisY.Step,
//                        AxisYNumber = data.MapArea.AxisY.Number,
//                        Target = _transformation.ConvertCoordinateToAtdi(new Wgs84Coordinate { Longitude = pointCoord.Longitude, Latitude = pointCoord.Latitude }, data.Projection),
//                        Point = _transformation.ConvertCoordinateToAtdi(new Wgs84Coordinate { Longitude = data.BroadcastingAssignment.SiteParameters.Lon_Dec, Latitude = data.BroadcastingAssignment.SiteParameters.Lat_Dec }, data.Projection),
//                        Location = data.MapArea.LowerLeft
//                    };
//                    CalcHeff(profileArgs, data, taskContext, ref effectiveHeight);
//                }
//                else
//                {
//                    effectiveHeight += data.BroadcastingAssignment.AntennaCharacteristics.AglHeight_m;
//                }
//                //


//                // параметр нужно будет добавить через буффер
//                //List<LandSea> landSeaList = new List<LandSea>();
//                LandSea[] landSeaList = new LandSea[1];
//                bool h2aboveSea = false;

//                if (data.ClutterContent != null)
//                {
//                    LandSea landSea = new LandSea { land = 0, sea = 0 };
//                    // d < 50 => asitis
//                    if (d_km <= 50)
//                    {
//                        var profileArgs = new CalcProfileIndexersArgs
//                        {
//                            AxisXStep = data.MapArea.AxisX.Step,
//                            AxisYStep = data.MapArea.AxisY.Step,
//                            AxisYNumber = data.MapArea.AxisY.Number,
//                            Target = _transformation.ConvertCoordinateToAtdi(new Wgs84Coordinate { Longitude = data.TargetCoordinate.Longitude, Latitude = data.TargetCoordinate.Latitude }, data.Projection),
//                            Point = _transformation.ConvertCoordinateToAtdi(new Wgs84Coordinate { Longitude = data.BroadcastingAssignment.SiteParameters.Lon_Dec, Latitude = data.BroadcastingAssignment.SiteParameters.Lat_Dec }, data.Projection),
//                            Location = data.MapArea.LowerLeft
//                        };
//                        CalcLandSeaOnProfile(profileArgs, data, taskContext, d_km, ref landSea, ref h2aboveSea);

//                        landSeaList[0] = new LandSea
//                        {
//                            land = landSea.land,
//                            sea = landSea.sea
//                        };
//                    }
//                    // d > 50 => sum of points on azimuth
//                    else
//                    {
//                        PointEarthGeometric startPoint = new PointEarthGeometric { Longitude = data.BroadcastingAssignment.SiteParameters.Lon_Dec, Latitude = data.BroadcastingAssignment.SiteParameters.Lat_Dec };
//                        var halfPathPoint = this._earthGeometricService.CalculationCoordinateByLengthAndAzimuth(startPoint, d_km / 2.0, AzimuthToTarget);
//                        var profileArgs = new CalcProfileIndexersArgs
//                        {
//                            AxisXStep = data.MapArea.AxisX.Step,
//                            AxisYStep = data.MapArea.AxisY.Step,
//                            AxisYNumber = data.MapArea.AxisY.Number,
//                            Target = _transformation.ConvertCoordinateToAtdi(new Wgs84Coordinate { Longitude = halfPathPoint.Longitude, Latitude = halfPathPoint.Latitude }, data.Projection),
//                            Point = _transformation.ConvertCoordinateToAtdi(new Wgs84Coordinate { Longitude = data.BroadcastingAssignment.SiteParameters.Lon_Dec, Latitude = data.BroadcastingAssignment.SiteParameters.Lat_Dec }, data.Projection),
//                            Location = data.MapArea.LowerLeft
//                        };
//                        CalcLandSeaOnProfile(profileArgs, data, taskContext, d_km / 2.0, ref landSea, ref h2aboveSea);

//                        landSeaList[0] = new LandSea
//                        {
//                            land = landSea.land,
//                            sea = landSea.sea
//                        };

//                        profileArgs = new CalcProfileIndexersArgs
//                        {
//                            AxisXStep = data.MapArea.AxisX.Step,
//                            AxisYStep = data.MapArea.AxisY.Step,
//                            AxisYNumber = data.MapArea.AxisY.Number,
//                            Target = _transformation.ConvertCoordinateToAtdi(new Wgs84Coordinate { Longitude = data.TargetCoordinate.Longitude, Latitude = data.TargetCoordinate.Latitude }, data.Projection),
//                            Point = _transformation.ConvertCoordinateToAtdi(new Wgs84Coordinate { Longitude = halfPathPoint.Longitude, Latitude = halfPathPoint.Latitude }, data.Projection),
//                            Location = data.MapArea.LowerLeft
//                        };
//                        CalcLandSeaOnProfile(profileArgs, data, taskContext, d_km / 2.0, ref landSea, ref h2aboveSea);

//                        landSeaList[0].land += landSea.land;
//                        landSeaList[0].sea += landSea.sea;
//                    }
//                }
//                else
//                {
//                    landSeaList[0] = new LandSea { land = d_km, sea = 0 };
//                }



//                //double Level_dBm = data.Transmitter.MaxPower_dBm - data.Transmitter.Loss_dB + antennaGainD - lossResult.LossD_dB; ????????????

//                double FSfor1kW_dBuVm = 0;//ITU1546_6.Get_E(data.BroadcastingAssignment.AntennaCharacteristics.AglHeight_m, effectiveHeight, d_km, data.BroadcastingAssignment.EmissionCharacteristics.Freq_MHz, data.PropagationModel.Parameters.Time_pc, effectiveHeight, data.TargetAltitude_m, h2aboveSea, landSeaList.ToArray());

//                if (data.PropagationModel.MainBlock.ModelType == MainCalcBlockModelType.ITU1546_4)
//                {
//                    var calcFSArgs = new CalcFSArgs()
//                    {
//                        ha = data.BroadcastingAssignment.AntennaCharacteristics.AglHeight_m,
//                        hef = effectiveHeight,
//                        d = d_km,
//                        f = data.BroadcastingAssignment.EmissionCharacteristics.Freq_MHz,
//                        p = data.PropagationModel.Parameters.Time_pc,
//                        h_gr = effectiveHeight,
//                        h2 = data.TargetAltitude_m,
//                        list1 = landSeaList.ToArray()
//                    };
//                    var calcFSResult = new CalcFSResult();
//                    _signalService.CalcFS_ITU1546_4(in calcFSArgs, ref calcFSResult);
//                    FSfor1kW_dBuVm = calcFSResult.FSResult;
//                    //FSfor1kW_dBuVm = ITU1546_4.Get_E(data.BroadcastingAssignment.AntennaCharacteristics.AglHeight_m, effectiveHeight, d_km, data.BroadcastingAssignment.EmissionCharacteristics.Freq_MHz, data.PropagationModel.Parameters.Time_pc, effectiveHeight, data.TargetAltitude_m, landSeaList.ToArray());
//                }
//                else if (data.PropagationModel.MainBlock.ModelType == MainCalcBlockModelType.ITU1546)
//                {
//                    //FSfor1kW_dBuVm = ITU1546_6.Get_E(data.BroadcastingAssignment.AntennaCharacteristics.AglHeight_m, effectiveHeight, d_km, data.BroadcastingAssignment.EmissionCharacteristics.Freq_MHz, data.PropagationModel.Parameters.Time_pc, effectiveHeight, data.TargetAltitude_m, h2aboveSea, landSeaList.ToArray());
//                    var calcFSArgs = new CalcFSArgs()
//                    {
//                        ha = data.BroadcastingAssignment.AntennaCharacteristics.AglHeight_m,
//                        hef = effectiveHeight,
//                        d = d_km,
//                        f = data.BroadcastingAssignment.EmissionCharacteristics.Freq_MHz,
//                        p = data.PropagationModel.Parameters.Time_pc,
//                        h_gr = effectiveHeight,
//                        h2 = data.TargetAltitude_m,
//                        h2AboveSea = h2aboveSea,
//                        list1 = landSeaList.ToArray()
//                    };
//                    var calcFSResult = new CalcFSResult();
//                    _signalService.CalcFS_ITU1546_ge06(in calcFSArgs, ref calcFSResult);
//                    FSfor1kW_dBuVm = calcFSResult.FSResult;
//                }
//                else if (data.PropagationModel.MainBlock.ModelType == MainCalcBlockModelType.ITU1546_ge06)
//                {
//                    //FSfor1kW_dBuVm = ITU1546_6.Get_E(data.BroadcastingAssignment.AntennaCharacteristics.AglHeight_m, effectiveHeight, d_km, data.BroadcastingAssignment.EmissionCharacteristics.Freq_MHz, data.PropagationModel.Parameters.Time_pc, effectiveHeight, data.TargetAltitude_m, h2aboveSea, landSeaList.ToArray());
//                    var calcFSArgs = new CalcFSArgs()
//                    {
//                        ha = data.BroadcastingAssignment.AntennaCharacteristics.AglHeight_m,
//                        hef = effectiveHeight,
//                        d = d_km,
//                        f = data.BroadcastingAssignment.EmissionCharacteristics.Freq_MHz,
//                        p = data.PropagationModel.Parameters.Time_pc,
//                        h_gr = effectiveHeight,
//                        h2 = data.TargetAltitude_m,
//                        h2AboveSea = h2aboveSea,
//                        list1 = landSeaList.ToArray()
//                    };
//                    var calcFSResult = new CalcFSResult();
//                    _signalService.CalcFS_ITU1546_ge06(in calcFSArgs, ref calcFSResult);
//                    FSfor1kW_dBuVm = calcFSResult.FSResult;
//                }
//                //var fS_dBuVm = FSfor1kW_dBuVm + ERP_dBW - 30;
//                //var level_dBm = ERP_dBW + 30 + fS_dBuVm - 139.3 - 20 * Math.Log10(data.BroadcastingAssignment.EmissionCharacteristics.Freq_MHz);

//                return new BroadcastingFieldStrengthCalcResult
//                {
//                    FS_dBuVm = FSfor1kW_dBuVm + ERP_dBW - 30,
//                    Level_dBm = FSfor1kW_dBuVm + ERP_dBW - 107.2 - 20 * Math.Log10(data.BroadcastingAssignment.EmissionCharacteristics.Freq_MHz)
//                    //AntennaPatternLoss_dB = antennaPatternLoss_dB
//                };
//            }
//            catch (Exception)
//            {
//                throw;
//            }

//        }

//        private static ProfileOptions DefineProfileOptions(PropagationModel propagModel)
//        {
//            var options = ProfileOptions.Empty;

//            options.Apply(MainBlockProfileOptions[propagModel.MainBlock.ModelType]);

//            if (propagModel.AbsorptionBlock.Available)
//            {
//                options.Apply(AbsorptionBlockProfileOptions[propagModel.AbsorptionBlock.ModelType]);
//            }
//            if (propagModel.AdditionalBlock.Available)
//            {
//                options.Apply(AdditionalBlockProfileOptions[propagModel.AdditionalBlock.ModelType]);
//            }
//            if (propagModel.AtmosphericBlock.Available)
//            {
//                options.Apply(AtmosphericBlockProfileOptions[propagModel.AtmosphericBlock.ModelType]);
//            }
//            if (propagModel.ClutterBlock.Available)
//            {
//                options.Apply(ClutterBlockProfileOptions[propagModel.ClutterBlock.ModelType]);
//            }
//            if (propagModel.DiffractionBlock.Available)
//            {
//                options.Apply(DiffractionBlockProfileOptions[propagModel.DiffractionBlock.ModelType]);
//            }
//            if (propagModel.SubPathDiffractionBlock.Available)
//            {
//                options.Apply(SubPathDiffractionBlockProfileOptions[propagModel.SubPathDiffractionBlock.ModelType]);
//            }
//            if (propagModel.DuctingBlock.Available)
//            {
//                options.Apply(DuctingBlockProfileOptions);
//            }
//            if (propagModel.ReflectionBlock.Available)
//            {
//                options.Apply(ReflectionBlockProfileOptions);
//            }
//            if (propagModel.TropoBlock.Available)
//            {
//                options.Apply(TropoBlockProfileOptions[propagModel.TropoBlock.ModelType]);
//            }
//            return options;
//        }

//    }
//}
