using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.Platform.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer.Internal;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.Gis.MapService;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    public class FieldStrengthCalcIterationRxPattern : IIterationHandler<FieldStrengthCalcData, FieldStrengthCalcResult>
    {
        private readonly ISignalService _signalService;
        private readonly IMapService _mapService;
        private readonly IObjectPoolSite _poolSite;
        private readonly IObjectPool<ProfileIndexer[]> _indexerArrayPool;
        private readonly IObjectPool<byte[]> _clutterArrayPool;
        private readonly IObjectPool<byte[]> _buildingArrayPool;
        private readonly IObjectPool<short[]> _reliefArrayPool;
        private readonly IObjectPool<short[]> _heightArrayPool;
        private readonly IEarthGeometricService _earthGeometricService;
        //private static int _dIndex;
        //private static int _dYIndex;
        //private static int _daxisXNumber;
        //private static int _dXIndex;


        #region Profile Config 
        private static readonly Dictionary<MainCalcBlockModelType, ProfileOptions> MainBlockProfileOptions = new Dictionary<MainCalcBlockModelType, ProfileOptions>
        {
            [MainCalcBlockModelType.Unknown] = ProfileOptions.Empty,
            [MainCalcBlockModelType.ITU2001] = ProfileOptions.OnlyHeight,
            [MainCalcBlockModelType.ITU1546] = ProfileOptions.ReliefAndClutter,
            [MainCalcBlockModelType.ITU525] = ProfileOptions.Empty
        };

        private static readonly Dictionary<AbsorptionCalcBlockModelType, ProfileOptions> AbsorptionBlockProfileOptions = new Dictionary<AbsorptionCalcBlockModelType, ProfileOptions>
        {
            [AbsorptionCalcBlockModelType.Unknown] = ProfileOptions.Empty,
            [AbsorptionCalcBlockModelType.Flat] = ProfileOptions.AllWithoutHeight,
            [AbsorptionCalcBlockModelType.FlatAndLinear] = ProfileOptions.AllWithoutHeight,
            [AbsorptionCalcBlockModelType.ITU2109AndLinear] = ProfileOptions.AllWithoutHeight,
            [AbsorptionCalcBlockModelType.ITU2109_2] = ProfileOptions.AllWithoutHeight,
            [AbsorptionCalcBlockModelType.Linear] = ProfileOptions.AllWithoutHeight,

        };

        private static readonly Dictionary<AdditionalCalcBlockModelType, ProfileOptions> AdditionalBlockProfileOptions = new Dictionary<AdditionalCalcBlockModelType, ProfileOptions>
        {
            [AdditionalCalcBlockModelType.Unknown] = ProfileOptions.Empty,
            [AdditionalCalcBlockModelType.ITU1820] = ProfileOptions.Empty

        };

        private static readonly Dictionary<AtmosphericCalcBlockModelType, ProfileOptions> AtmosphericBlockProfileOptions = new Dictionary<AtmosphericCalcBlockModelType, ProfileOptions>
        {
            [AtmosphericCalcBlockModelType.Unknown] = ProfileOptions.Empty,
            [AtmosphericCalcBlockModelType.ITU1820] = ProfileOptions.Empty,
            [AtmosphericCalcBlockModelType.ITU838_530] = ProfileOptions.OnlyHeight,
            [AtmosphericCalcBlockModelType.ITU678] = ProfileOptions.Empty,
        };

        private static readonly Dictionary<ClutterCalcBlockModelType, ProfileOptions> ClutterBlockProfileOptions = new Dictionary<ClutterCalcBlockModelType, ProfileOptions>
        {
            [ClutterCalcBlockModelType.Unknown] = ProfileOptions.Empty,
            [ClutterCalcBlockModelType.ITU2109] = ProfileOptions.AllWithoutHeight,
            [ClutterCalcBlockModelType.Flat] = ProfileOptions.AllWithoutHeight
        };

        private static readonly Dictionary<DiffractionCalcBlockModelType, ProfileOptions> DiffractionBlockProfileOptions = new Dictionary<DiffractionCalcBlockModelType, ProfileOptions>
        {
            [DiffractionCalcBlockModelType.Unknown] = ProfileOptions.Empty,
            [DiffractionCalcBlockModelType.Bullington] = ProfileOptions.OnlyHeight,
            [DiffractionCalcBlockModelType.Deygout66] = ProfileOptions.OnlyHeight,
            [DiffractionCalcBlockModelType.Deygout91] = ProfileOptions.OnlyHeight,
            [DiffractionCalcBlockModelType.ITU526_15] = ProfileOptions.OnlyHeight,

        };

        private static readonly Dictionary<SubPathDiffractionCalcBlockModelType, ProfileOptions> SubPathDiffractionBlockProfileOptions = new Dictionary<SubPathDiffractionCalcBlockModelType, ProfileOptions>
        {
            [SubPathDiffractionCalcBlockModelType.Unknown] = ProfileOptions.Empty,
            [SubPathDiffractionCalcBlockModelType.SubDeygout91] = ProfileOptions.Empty
        };

        private static readonly Dictionary<TropoCalcBlockModelType, ProfileOptions> TropoBlockProfileOptions = new Dictionary<TropoCalcBlockModelType, ProfileOptions>
        {
            [TropoCalcBlockModelType.Unknown] = ProfileOptions.Empty,
            [TropoCalcBlockModelType.ITU617] = ProfileOptions.OnlyHeight
        };

        private static readonly ProfileOptions DuctingBlockProfileOptions = ProfileOptions.OnlyHeight;

        private static readonly ProfileOptions ReflectionBlockProfileOptions = ProfileOptions.Empty;

        private struct ProfileOptions
        {
            public static ProfileOptions Empty = new ProfileOptions();

            public static ProfileOptions AllWithoutHeight = new ProfileOptions
            {
                Relief = true,
                Clutter = true,
                Building = true,
                Height = false
            };

            public static ProfileOptions OnlyHeight = new ProfileOptions
            {
                Relief = false,
                Clutter = false,
                Building = false,
                Height = true
            };

            public static ProfileOptions ReliefAndClutter = new ProfileOptions
            {
                Relief = true,
                Clutter = true,
                Building = false,
                Height = false
            };

            public bool Clutter;
            public bool Relief;
            public bool Building;
            public bool Height;

            public bool IsNotEmpty => Relief || Clutter || Building || Height;

            public void Apply(ProfileOptions source)
            {
                if (source.Building)
                {
                    this.Building = true;
                }
                if (source.Clutter)
                {
                    this.Clutter = true;
                }
                if (source.Height)
                {
                    this.Height = true;
                }
                if (source.Relief)
                {
                    this.Relief = true;
                }
            }
        }

        #endregion

        public FieldStrengthCalcIterationRxPattern(
            ISignalService signalService,
            IEarthGeometricService earthGeometricService,
            IMapService mapService,
            IObjectPoolSite poolSite)
        {
            _signalService = signalService;
            _mapService = mapService;
            _poolSite = poolSite;
            _earthGeometricService = earthGeometricService;

            _indexerArrayPool = _poolSite.GetPool<ProfileIndexer[]>(ObjectPools.GisProfileIndexerArrayObjectPool);
            _clutterArrayPool = _poolSite.GetPool<byte[]>(ObjectPools.GisProfileClutterArrayObjectPool);
            _buildingArrayPool = _poolSite.GetPool<byte[]>(ObjectPools.GisProfileBuildingArrayObjectPool);
            _reliefArrayPool = _poolSite.GetPool<short[]>(ObjectPools.GisProfileReliefArrayObjectPool);
            _heightArrayPool = _poolSite.GetPool<short[]>(ObjectPools.GisProfileHeightArrayObjectPool);
        }

        public FieldStrengthCalcResult Run(ITaskContext taskContext, FieldStrengthCalcData data)
        {
            var profileOptions = DefineProfileOptions(data.PropagationModel);

            var indexerBuffer = default(ProfileIndexer[]);
            var clutterBuffer = default(byte[]);
            var buildingBuffer = default(byte[]);
            var reliefBuffer = default(short[]);
            var heightBuffer = default(short[]);
            var profileLenght = 0;

            try
            {
                if (profileOptions.IsNotEmpty)
                {
                    //  получаем буффер под профиль
                    indexerBuffer = _indexerArrayPool.Take();

                    // расчитываем профиль
                    var profileArgs = new CalcProfileIndexersArgs
                    {
                        AxisXStep = data.MapArea.AxisX.Step,
                        AxisYStep = data.MapArea.AxisY.Step,
                        AxisYNumber = data.MapArea.AxisY.Number,
                        Target = data.TargetCoordinate,
                        Point = data.PointCoordinate,
                        Location = data.MapArea.LowerLeft
                    };
                    var profileResult = new CalcProfileIndexersResult
                    {
                        Indexers = indexerBuffer,
                        StartPosition = 0,
                        IndexerCount = 0
                    };
                    _mapService.CalcProfileIndexers(in profileArgs, ref profileResult);
                    profileLenght = profileResult.IndexerCount;

                    // получаем  буферы массивов
                    if (profileOptions.Relief)
                    {
                        reliefBuffer = _reliefArrayPool.Take();
                    }
                    if (profileOptions.Building)
                    {
                        buildingBuffer = _buildingArrayPool.Take();
                    }
                    if (profileOptions.Clutter)
                    {
                        clutterBuffer = _clutterArrayPool.Take();
                    }
                    if (profileOptions.Height)
                    {
                        heightBuffer = _heightArrayPool.Take();
                    }
                    // заполняем профель данными
                    var axisXNumber = data.MapArea.AxisX.Number;
                    for (int i = 0; i < profileResult.IndexerCount; i++)
                    {
                        var indexer = indexerBuffer[i];
                        var contentIndex = indexer.YIndex * axisXNumber + indexer.XIndex;
                        //_dIndex = contentIndex;
                        //_dYIndex = indexer.YIndex;
                        //_daxisXNumber = axisXNumber;
                        //_dXIndex = indexer.XIndex;




                        var clutterValue = MapSpecification.DefaultForClutter;
                        int clutterH = 0;
                        if (profileOptions.Height || profileOptions.Clutter)
                        {
                            clutterValue = data.ClutterContent[contentIndex];
                            if (profileOptions.Clutter)
                            {
                                clutterBuffer[i] = clutterValue;

                                if (data.CluttersDesc.Frequencies != null && data.CluttersDesc.Frequencies.Length > 0 &&
                                    data.CluttersDesc.Frequencies[0].Clutters != null)
                                {
                                    clutterH = data.CluttersDesc.Frequencies[0].Clutters[clutterValue].Height_m;// добавить проверку что это существует.
                                }

                            }
                        }

                        var buildingValue = MapSpecification.DefaultForBuilding;
                        if (profileOptions.Height || profileOptions.Building)
                        {
                            buildingValue = data.BuildingContent[contentIndex];
                            if (profileOptions.Building)
                            {
                                buildingBuffer[i] = buildingValue;
                            }
                        }

                        var reliefValue = MapSpecification.DefaultForRelief;
                        if (profileOptions.Height || profileOptions.Relief)
                        {
                            reliefValue = data.ReliefContent[contentIndex];
                            if (profileOptions.Relief)
                            {
                                reliefBuffer[i] = reliefValue;
                            }
                        }

                        if (profileOptions.Height)
                        {
                            if (buildingValue != MapSpecification.DefaultForBuilding)
                            {
                                heightBuffer[i] = (short)(reliefValue + buildingValue);
                            }
                            else
                            {
                                heightBuffer[i] = (short)(reliefValue + clutterH);
                            }
                        }
                    }
                }
                var pointSourceArgs = new PointEarthGeometric() { Longitude = data.PointCoordinate.X, Latitude = data.PointCoordinate.Y, CoordinateUnits = CoordinateUnits.m };
                var pointTargetArgs = new PointEarthGeometric() { Longitude = data.TargetCoordinate.X, Latitude = data.TargetCoordinate.Y, CoordinateUnits = CoordinateUnits.m };
                var d_km = this._earthGeometricService.GetDistance_km(in pointSourceArgs, in pointTargetArgs);
                var lossArgs = new CalcLossArgs
                {
                    Model = data.PropagationModel,
                    BuildingProfile = buildingBuffer,
                    BuildingStartIndex = 0,
                    ClutterProfile = clutterBuffer,
                    ClutterStartIndex = 0,
                    HeightProfile = heightBuffer,
                    HeightStartIndex = 0,
                    ReliefProfile = reliefBuffer,
                    ReliefStartIndex = 0,
                    ProfileLength = profileLenght,
                    Freq_Mhz = data.Transmitter.Freq_MHz,
                    D_km = d_km,
                    Ha_m = data.PointAltitude_m,
                    Hb_m = data.TargetAltitude_m,
                    CluttersDesc = data.CluttersDesc
                };

                var lossResult = new CalcLossResult();
                _signalService.CalcLoss(in lossArgs, ref lossResult);
                var azimutToTargetRx = this._earthGeometricService.GetAzimut(in pointSourceArgs, in pointTargetArgs);
                var txAntennaGainArgs = new CalcAntennaGainArgs
                {
                    Antenna = data.Antenna,
                    AzimutToTarget_deg = azimutToTargetRx,
                    TiltToTarget_deg = lossResult.TiltaD_Deg,
                    PolarizationEquipment = data.Transmitter.Polarization,
                    PolarizationWave = data.Transmitter.Polarization
                };
                var txAntennaGainD = _signalService.CalcAntennaGain(in txAntennaGainArgs);
                //double Level_dBm = data.Transmitter.MaxPower_dBm - data.Transmitter.Loss_dB + txAntennaGainD - lossResult.LossD_dB;
                double txAntennaPatternLoss_dB = txAntennaGainD - txAntennaGainArgs.Antenna.Gain_dB;

                //// Rx ant gain
                var azimutToTargetTx = this._earthGeometricService.GetAzimut(in pointTargetArgs, in pointSourceArgs);
                var rxAntennaGainArgs = new CalcAntennaGainArgs
                {
                    Antenna = data.Antenna,
                    AzimutToTarget_deg = azimutToTargetTx,
                    TiltToTarget_deg = lossResult.TiltaD_Deg,
                    PolarizationEquipment = data.Transmitter.Polarization,
                    PolarizationWave = data.Transmitter.Polarization
                };
                var rxAntennaGainD = _signalService.CalcAntennaGain(in rxAntennaGainArgs);
                double Level_dBm = data.Transmitter.MaxPower_dBm - data.Transmitter.Loss_dB + txAntennaGainD - lossResult.LossD_dB + rxAntennaGainD;
                double rxAntennaPatternLoss_dB = txAntennaGainD - rxAntennaGainArgs.Antenna.Gain_dB;
                //// 
                if (data.PropagationModel.AbsorptionBlock.Available)
                {// нужен учет дополнительного пути распространения
                    double Loss1 = lossResult.LossD_dB - txAntennaGainD;
                    txAntennaGainArgs.TiltToTarget_deg = lossResult.TiltaA_Deg;
                    var antennaGainA = _signalService.CalcAntennaGain(in txAntennaGainArgs);

                    double Loss2 = lossResult.LossA_dB - antennaGainA;
                    double LossSum = Loss1;

                    if (Loss1 > Loss2)
                    {
                        LossSum = Loss2;
                        txAntennaPatternLoss_dB = antennaGainA - txAntennaGainArgs.Antenna.Gain_dB;
                    }
                    //double LossSum = Math.Min(Loss1, Loss2);
                    //LossSum = LossSum - 10 * Math.Log10(Math.Pow(10, -0.1 * (Loss1 - LossSum)) + Math.Pow(10, -0.1 * (Loss2 - LossSum)));
                    Level_dBm = data.Transmitter.MaxPower_dBm - data.Transmitter.Loss_dB - LossSum;
                }
                double FS_dBuVm = Level_dBm + 77.2 + 20 * Math.Log10(data.Transmitter.Freq_MHz);
                return new FieldStrengthCalcResult
                {
                    FS_dBuVm = FS_dBuVm,
                    Level_dBm = Level_dBm,
                    AntennaPatternLoss_dB = - txAntennaPatternLoss_dB
                };
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (indexerBuffer != null)
                {
                    _indexerArrayPool.Put(indexerBuffer);
                }
                if (clutterBuffer != null)
                {
                    _clutterArrayPool.Put(clutterBuffer);
                }
                if (buildingBuffer != null)
                {
                    _buildingArrayPool.Put(buildingBuffer);
                }
                if (reliefBuffer != null)
                {
                    _reliefArrayPool.Put(reliefBuffer);
                }
                if (heightBuffer != null)
                {
                    _heightArrayPool.Put(heightBuffer);
                }
            }
        }

        private static ProfileOptions DefineProfileOptions(PropagationModel propagModel)
        {
            var options = ProfileOptions.Empty;

            options.Apply(MainBlockProfileOptions[propagModel.MainBlock.ModelType]);

            if (propagModel.AbsorptionBlock.Available)
            {
                options.Apply(AbsorptionBlockProfileOptions[propagModel.AbsorptionBlock.ModelType]);
            }
            if (propagModel.AdditionalBlock.Available)
            {
                options.Apply(AdditionalBlockProfileOptions[propagModel.AdditionalBlock.ModelType]);
            }
            if (propagModel.AtmosphericBlock.Available)
            {
                options.Apply(AtmosphericBlockProfileOptions[propagModel.AtmosphericBlock.ModelType]);
            }
            if (propagModel.ClutterBlock.Available)
            {
                options.Apply(ClutterBlockProfileOptions[propagModel.ClutterBlock.ModelType]);
            }
            if (propagModel.DiffractionBlock.Available)
            {
                options.Apply(DiffractionBlockProfileOptions[propagModel.DiffractionBlock.ModelType]);
            }
            if (propagModel.SubPathDiffractionBlock.Available)
            {
                options.Apply(SubPathDiffractionBlockProfileOptions[propagModel.SubPathDiffractionBlock.ModelType]);
            }
            if (propagModel.DuctingBlock.Available)
            {
                options.Apply(DuctingBlockProfileOptions);
            }
            if (propagModel.ReflectionBlock.Available)
            {
                options.Apply(ReflectionBlockProfileOptions);
            }
            if (propagModel.TropoBlock.Available)
            {
                options.Apply(TropoBlockProfileOptions[propagModel.TropoBlock.ModelType]);
            }
            return options;
        }

    }
}
