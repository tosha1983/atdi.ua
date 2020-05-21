using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.CalcServer.Internal;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using IC = Atdi.DataModels.Sdrn.Infocenter.Entities;
using Atdi.Platform.Logging;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;
using Atdi.DataModels.Sdrn.Infocenter.Entities.Entities.SdrnServer;
using Atdi.Platform;
using Atdi.Common;
using Atdi.Common.Extensions;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks
{
	[TaskHandler(CalcTaskType.StationCalibrationCalcTask)]
	public class StationCalibrationCalcTask : ITaskHandler
	{
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
        private readonly IDataLayer<EntityDataOrm<IC.InfocenterEntityOrmContext>> _infocenterDataLayer;
        private readonly IClientContextService _contextService;
		private readonly IMapRepository _mapRepository;
		private readonly IIterationsPool _iterationsPool;
        private readonly ITransformation _transformation;
		private readonly ILogger _logger;
        private readonly AppServerComponentConfig _appServerComponentConfig;
        private ITaskContext _taskContext;
		private IDataLayerScope _calcDbScope;
		private TaskParameters _parameters;
		private ContextStation[] _contextStations;
        private DriveTestsResult[] _contextDriveTestsResult;
        private PropagationModel _propagationModel;
        private ProjectMapData _mapData;
		private CluttersDesc _cluttersDesc;
        



        private class TaskParameters
		{
            public long[] InfocMeasResults;

            public long[] StationIds;

            public CalibrationParameters CalibrationParameters;

            public CorellationParameters CorellationParameters;

            public GeneralParameters  GeneralParameters;

            public string Projection;

			public string MapName;
		}

		public StationCalibrationCalcTask(
			IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
			IClientContextService contextService,
			IMapRepository mapRepository,
			IIterationsPool iterationsPool,
			ITransformation transformation,
            AppServerComponentConfig appServerComponentConfig,
            IDataLayer<EntityDataOrm<IC.InfocenterEntityOrmContext>> infocenterDataLayer,
            ILogger logger)
		{
			_calcServerDataLayer = calcServerDataLayer;
			_contextService = contextService;
			_mapRepository = mapRepository;
			_iterationsPool = iterationsPool;
			_transformation = transformation;
            _appServerComponentConfig = appServerComponentConfig;
            _infocenterDataLayer = infocenterDataLayer;
            _logger = logger;
		}

		public void Dispose()
		{
			if (_calcDbScope != null)
			{
				_calcDbScope.Dispose();
				_calcDbScope = null;
			}

			_taskContext = null;
		}

        public void Load(ITaskContext taskContext)
        {

            this._taskContext = taskContext;

            this._calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>();

            // загрузить параметры задачи
            this.LoadTaskParameters();
            this.ValidateTaskParameters();

            this._propagationModel = _contextService.GetPropagationModel(this._calcDbScope, taskContext.ClientContextId);

           
            //заполнение this._contextDriveTestsResult (из инфоцентра)

            // найти и загрузить карту
            this._mapData = _mapRepository.GetMapByName(this._calcDbScope, this._taskContext.ProjectId, this._parameters.MapName);
            this._cluttersDesc = _mapRepository.GetCluttersDesc(this._calcDbScope, this._mapData.Id);
        }


        public void Run()
        {
            var fieldStrengthCalcDatas = new FieldStrengthCalcData[this._contextStations.Length];
            for (int i = 0; i < this._contextStations.Length; i++)
            {
                fieldStrengthCalcDatas[i] = new FieldStrengthCalcData
                {
                    Antenna = this._contextStations[i].Antenna,
                    PropagationModel = _propagationModel,
                    PointCoordinate = this._contextStations[i].Coordinate,
                    PointAltitude_m = this._contextStations[i].Site.Altitude,
                    MapArea = _mapData.Area,
                    BuildingContent = _mapData.BuildingContent,
                    ClutterContent = _mapData.ClutterContent,
                    ReliefContent = _mapData.ReliefContent,
                    Transmitter = this._contextStations[i].Transmitter,
                    CluttersDesc = _cluttersDesc
                };
            }
            var iterationAllStationCorellationCalcData = new AllStationCorellationCalcData
            {
                GSIDGroupeStation = this._contextStations,
                CalibrationParameters = this._parameters.CalibrationParameters,
                CorellationParameters = this._parameters.CorellationParameters,
                GSIDGroupeDriveTests = this._contextDriveTestsResult,
                FieldStrengthCalcData = fieldStrengthCalcDatas,
                GeneralParameters = this._parameters.GeneralParameters
            };

            var iterationResultCalibration = _iterationsPool.GetIteration<AllStationCorellationCalcData, CalibrationResult>();
            var resulCalibration = iterationResultCalibration.Run(_taskContext, iterationAllStationCorellationCalcData);
        }

		private void ValidateTaskParameters()
		{
			if (this._parameters == null)
			{
				throw new InvalidOperationException("Undefined a task parameters");
			}
		}

        private void LoadTaskParameters()
        {
            // load parameters
            var query = _calcServerDataLayer.GetBuilder<IStationCalibrationArgs>()
                            .From()
                            .Select(
                                c => c.AltitudeStation,
                                c => c.AzimuthStation,
                                c => c.CascadeTuning,
                                c => c.CoordinatesStation,
                                c => c.CorrelationDistance_m,
                                c => c.Delta_dB,
                                c => c.Detail,
                                c => c.DetailOfCascade,
                                c => c.MaxAntennasPatternLoss_dB,
                                c => c.MaxDeviationAltitudeStation_m,
                                c => c.MaxDeviationAzimuthStation_deg,
                                c => c.MaxDeviationCoordinatesStation_m,
                                c => c.MaxDeviationTiltStation_deg,
                                c => c.MaxRangeMeasurements_dBmkV,
                                c => c.Method,
                                c => c.MinRangeMeasurements_dBmkV,
                                c => c.NumberCascade,
                                c => c.PowerStation,
                                c => c.ShiftAltitudeStationMax_m,
                                c => c.ShiftAltitudeStationMin_m,
                                c => c.ShiftAltitudeStationStep_m,
                                c => c.ShiftAzimuthStationMax_deg,
                                c => c.ShiftAzimuthStationMin_deg,
                                c => c.ShiftAzimuthStationStep_deg,
                                c => c.ShiftCoordinatesStationStep_m,
                                c => c.ShiftCoordinatesStation_m,
                                c => c.ShiftPowerStationMax_dB,
                                c => c.ShiftPowerStationMin_dB,
                                c => c.ShiftPowerStationStep_dB,
                                c => c.ShiftTiltStationMax_deg,
                                c => c.ShiftTiltStationMin_deg,
                                c => c.ShiftTiltStationStep_deg,
                                c => c.TiltStation,
                                c => c.TASK.CONTEXT.PROJECT.Projection,
                                c => c.TASK.MapName,
                                c => c.DistanceAroundContour_km,
                                c => c.MinNumberPointForCorrelation,
                                c => c.TrustOldResults,
                                c => c.UseMeasurementSameGSID,
                                c => c.СorrelationThresholdHard,
                                c => c.СorrelationThresholdWeak,
                                c => c.InfocMeasResults,
                                c => c.StationIds
                            )
                            .Where(c => c.TaskId, ConditionOperator.Equal, _taskContext.TaskId);

            this._parameters = _calcDbScope.Executor.ExecuteAndFetch(query, reader =>
            {
                if (!reader.Read())
                {
                    return null;
                }

                var method = Method.ExhaustiveSearch;
                switch (reader.GetValue(c => c.Method).GetValueOrDefault())
                {
                    case 0:
                        method = Method.ExhaustiveSearch;
                        break;
                    case 1:
                        method = Method.QuickDescent;
                        break;
                }

                return new TaskParameters()
                {
                    GeneralParameters = new GeneralParameters()
                    {
                        DistanceAroundContour_km = reader.GetValue(c => c.DistanceAroundContour_km).GetValueOrDefault(),
                        MinNumberPointForCorrelation = reader.GetValue(c => c.MinNumberPointForCorrelation).GetValueOrDefault(),
                        TrustOldResults = reader.GetValue(c => c.TrustOldResults).GetValueOrDefault(),
                        UseMeasurementSameGSID = reader.GetValue(c => c.UseMeasurementSameGSID).GetValueOrDefault(),
                        СorrelationThresholdHard = reader.GetValue(c => c.СorrelationThresholdHard).GetValueOrDefault(),
                        СorrelationThresholdWeak = reader.GetValue(c => c.СorrelationThresholdWeak).GetValueOrDefault()
                    },
                    CorellationParameters = new CorellationParameters()
                    {
                        CorrelationDistance_m = reader.GetValue(c => c.CorrelationDistance_m).GetValueOrDefault(),
                        Delta_dB = reader.GetValue(c => c.Delta_dB).GetValueOrDefault(),
                        Detail = reader.GetValue(c => c.Detail).GetValueOrDefault(),
                        MaxAntennasPatternLoss_dB = reader.GetValue(c => c.MaxAntennasPatternLoss_dB).GetValueOrDefault(),
                        MaxRangeMeasurements_dBmkV = reader.GetValue(c => c.MaxRangeMeasurements_dBmkV).GetValueOrDefault(),
                        MinRangeMeasurements_dBmkV = reader.GetValue(c => c.MinRangeMeasurements_dBmkV).GetValueOrDefault()
                    },
                    CalibrationParameters = new CalibrationParameters()
                    {
                        AltitudeStation = reader.GetValue(c => c.AltitudeStation).GetValueOrDefault(),
                        AzimuthStation = reader.GetValue(c => c.AzimuthStation).GetValueOrDefault(),
                        CascadeTuning = reader.GetValue(c => c.CascadeTuning).GetValueOrDefault(),
                        CoordinatesStation = reader.GetValue(c => c.CoordinatesStation).GetValueOrDefault(),
                        DetailOfCascade = reader.GetValue(c => c.DetailOfCascade).GetValueOrDefault(),
                        MaxDeviationAltitudeStation_m = reader.GetValue(c => c.MaxDeviationAltitudeStation_m).GetValueOrDefault(),
                        MaxDeviationAzimuthStation_deg = reader.GetValue(c => c.MaxDeviationAzimuthStation_deg).GetValueOrDefault(),
                        MaxDeviationCoordinatesStation_m = reader.GetValue(c => c.MaxDeviationCoordinatesStation_m).GetValueOrDefault(),
                        MaxDeviationTiltStationDeg = reader.GetValue(c => c.MaxDeviationTiltStation_deg).GetValueOrDefault(),
                        Method = method,
                        NumberCascade = reader.GetValue(c => c.NumberCascade).GetValueOrDefault(),
                        PowerStation = reader.GetValue(c => c.PowerStation).GetValueOrDefault(),
                        ShiftAltitudeStationMax_m = reader.GetValue(c => c.ShiftAltitudeStationMax_m).GetValueOrDefault(),
                        ShiftAltitudeStationMin_m = reader.GetValue(c => c.ShiftAltitudeStationMin_m).GetValueOrDefault(),
                        ShiftAltitudeStationStep_m = reader.GetValue(c => c.ShiftAltitudeStationStep_m).GetValueOrDefault(),
                        ShiftAzimuthStationMax_deg = reader.GetValue(c => c.ShiftAzimuthStationMax_deg).GetValueOrDefault(),
                        ShiftAzimuthStationMin_deg = reader.GetValue(c => c.ShiftAzimuthStationMin_deg).GetValueOrDefault(),
                        ShiftAzimuthStationStep_deg = reader.GetValue(c => c.ShiftAzimuthStationStep_deg).GetValueOrDefault(),
                        ShiftCoordinatesStationStep_m = reader.GetValue(c => c.ShiftCoordinatesStationStep_m).GetValueOrDefault(),
                        ShiftCoordinatesStation_m = reader.GetValue(c => c.ShiftCoordinatesStation_m).GetValueOrDefault(),
                        ShiftPowerStationMax_dB = reader.GetValue(c => c.ShiftPowerStationMax_dB).GetValueOrDefault(),
                        ShiftPowerStationMin_dB = reader.GetValue(c => c.ShiftPowerStationMin_dB).GetValueOrDefault(),
                        ShiftPowerStationStep_dB = reader.GetValue(c => c.ShiftPowerStationStep_dB).GetValueOrDefault(),
                        ShiftTiltStationMax_Deg = reader.GetValue(c => c.ShiftTiltStationMax_deg).GetValueOrDefault(),
                        ShiftTiltStationMin_Deg = reader.GetValue(c => c.ShiftTiltStationMin_deg).GetValueOrDefault(),
                        ShiftTiltStationStep_Deg = reader.GetValue(c => c.ShiftTiltStationStep_deg).GetValueOrDefault(),
                        TiltStation = reader.GetValue(c => c.TiltStation).GetValueOrDefault()
                    },
                    Projection = reader.GetValue(c => c.TASK.CONTEXT.PROJECT.Projection),
                    MapName = reader.GetValue(c => c.TASK.MapName),
                    StationIds = reader.GetValue(c => c.StationIds),
                    InfocMeasResults = reader.GetValue(c => c.InfocMeasResults)
                };
            });


            // load stations
            var clientContextStations = new List<ContextStation>();

            var partStationIdsByCalcServer = BreakDownElemBlocks.BreakDown(this._parameters.StationIds);
            for (int i = 0; i < partStationIdsByCalcServer.Count; i++)
            {
                var queryStation = _calcServerDataLayer.GetBuilder<IContextStation>()
                    .From()
                    .Select(
                        c => c.Id,
                        c => c.StationIdByICSM,
                        c => c.TableNameByICSM,
                        c => c.CONTEXT.Id,
                        c => c.StateCode,
                        c => c.CreatedDate,
                        c => c.CallSign,
                        c => c.Name,
                        c => c.Standard,
                        c => c.ModifiedDate,

                        c => c.SITE.Latitude_DEC,
                        c => c.SITE.Longitude_DEC,
                        c => c.SITE.Altitude_m,

                        c => c.COORDINATES.AtdiX,
                        c => c.COORDINATES.AtdiY,

                        c => c.ANTENNA.Azimuth_deg,
                        c => c.ANTENNA.Gain_dB,
                        c => c.ANTENNA.ItuPatternCode,
                        c => c.ANTENNA.Tilt_deg,
                        c => c.ANTENNA.XPD_dB,

                        c => c.ANTENNA.HH_PATTERN.StationId,
                        c => c.ANTENNA.HH_PATTERN.Angle_deg,
                        c => c.ANTENNA.HH_PATTERN.Loss_dB,

                        c => c.ANTENNA.HV_PATTERN.StationId,
                        c => c.ANTENNA.HV_PATTERN.Angle_deg,
                        c => c.ANTENNA.HV_PATTERN.Loss_dB,

                        c => c.ANTENNA.VH_PATTERN.StationId,
                        c => c.ANTENNA.VH_PATTERN.Angle_deg,
                        c => c.ANTENNA.VH_PATTERN.Loss_dB,

                        c => c.ANTENNA.VV_PATTERN.StationId,
                        c => c.ANTENNA.VV_PATTERN.Angle_deg,
                        c => c.ANTENNA.VV_PATTERN.Loss_dB,

                        c => c.TRANSMITTER.StationId,
                        c => c.TRANSMITTER.Loss_dB,
                        c => c.TRANSMITTER.Freq_MHz,
                        c => c.TRANSMITTER.BW_kHz,
                        c => c.TRANSMITTER.MaxPower_dBm,
                        c => c.TRANSMITTER.PolarizationCode,
                        c => c.TRANSMITTER.Freqs_MHz,


                        c => c.RECEIVER.StationId,
                        c => c.RECEIVER.Loss_dB,
                        c => c.RECEIVER.Freq_MHz,
                        c => c.RECEIVER.BW_kHz,
                        c => c.RECEIVER.KTBF_dBm,
                        c => c.RECEIVER.Threshold_dBm,
                        c => c.RECEIVER.PolarizationCode,
                        c => c.RECEIVER.Freqs_MHz
                    )
                    .Where(c => c.CONTEXT.Id, ConditionOperator.Equal, _taskContext.ClientContextId)
                    .Where(c => c.Id, ConditionOperator.In, partStationIdsByCalcServer[i].ToArray());

                var contextStation = _calcDbScope.Executor.ExecuteAndFetch(queryStation, reader =>
                {
                    while (reader.Read())
                    {

                        var stationRecord = new ContextStation
                        {
                            Id = reader.GetValue(c => c.Id),
                            ContextId = _taskContext.ClientContextId,
                            CreatedDate = reader.GetValue(c => c.CreatedDate),
                            Type = (ClientContextStationType)reader.GetValue(c => c.StateCode),
                            CallSign = reader.GetValue(c => c.CallSign),
                            Name = reader.GetValue(c => c.Name),
                            Standard = reader.GetValue(c => c.Standard),
                            StationIdByICSM = reader.GetValue(c => c.StationIdByICSM),
                            TableNameByICSM = reader.GetValue(c => c.TableNameByICSM),
                            ModifiedDate = reader.GetValue(c => c.ModifiedDate),
                            Site = new Wgs84Site
                            {
                                Latitude = reader.GetValue(c => c.SITE.Latitude_DEC),
                                Longitude = reader.GetValue(c => c.SITE.Longitude_DEC),
                                Altitude = reader.GetValue(c => c.SITE.Altitude_m)
                            },
                            Coordinate = new AtdiCoordinate
                            {
                                X = reader.GetValue(c => c.COORDINATES.AtdiX),
                                Y = reader.GetValue(c => c.COORDINATES.AtdiY)
                            },
                            Antenna = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntenna
                            {
                                Gain_dB = reader.GetValue(c => c.ANTENNA.Gain_dB),
                                XPD_dB = reader.GetValue(c => c.ANTENNA.XPD_dB),
                                Azimuth_deg = reader.GetValue(c => c.ANTENNA.Azimuth_deg),
                                ItuPattern = (DataModels.Sdrn.DeepServices.RadioSystem.Stations.AntennaItuPattern)reader.GetValue(c => c.ANTENNA.ItuPatternCode),
                                Tilt_deg = reader.GetValue(c => c.ANTENNA.Tilt_deg)
                            },
                        };

                        if (reader.IsNotNull(c => c.TRANSMITTER.StationId))
                        {
                            stationRecord.Transmitter = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationTransmitter
                            {
                                Loss_dB = reader.GetValue(c => c.TRANSMITTER.Loss_dB),
                                Polarization = (DataModels.Sdrn.DeepServices.RadioSystem.Stations.PolarizationType)reader.GetValue(c => c.TRANSMITTER.PolarizationCode),
                                MaxPower_dBm = reader.GetValue(c => c.TRANSMITTER.MaxPower_dBm),
                                BW_kHz = reader.GetValue(c => c.TRANSMITTER.BW_kHz),
                                Freq_MHz = reader.GetValue(c => c.TRANSMITTER.Freq_MHz),
                                Freqs_MHz = reader.GetValue(c => c.TRANSMITTER.Freqs_MHz)
                            };
                        }

                        if (reader.IsNotNull(c => c.RECEIVER.StationId))
                        {
                            stationRecord.Receiver = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationReceiver
                            {
                                Loss_dB = reader.GetValue(c => c.RECEIVER.Loss_dB),
                                Polarization = (DataModels.Sdrn.DeepServices.RadioSystem.Stations.PolarizationType)reader.GetValue(c => c.RECEIVER.PolarizationCode),
                                KTBF_dBm = reader.GetValue(c => c.RECEIVER.KTBF_dBm),
                                BW_kHz = reader.GetValue(c => c.RECEIVER.BW_kHz),
                                Freq_MHz = reader.GetValue(c => c.RECEIVER.Freq_MHz),
                                Threshold_dBm = reader.GetValue(c => c.RECEIVER.Threshold_dBm),
                                Freqs_MHz = reader.GetValue(c => c.RECEIVER.Freqs_MHz)
                            };
                        }

                        if (reader.IsNotNull(c => c.ANTENNA.HH_PATTERN.StationId))
                        {
                            stationRecord.Antenna.HhPattern.Angle_deg = reader.GetValue(c => c.ANTENNA.HH_PATTERN.Angle_deg);
                            stationRecord.Antenna.HhPattern.Loss_dB = reader.GetValue(c => c.ANTENNA.HH_PATTERN.Loss_dB);
                        }
                        if (reader.IsNotNull(c => c.ANTENNA.HV_PATTERN.StationId))
                        {
                            stationRecord.Antenna.HvPattern.Angle_deg = reader.GetValue(c => c.ANTENNA.HV_PATTERN.Angle_deg);
                            stationRecord.Antenna.HvPattern.Loss_dB = reader.GetValue(c => c.ANTENNA.HV_PATTERN.Loss_dB);
                        }
                        if (reader.IsNotNull(c => c.ANTENNA.VH_PATTERN.StationId))
                        {
                            stationRecord.Antenna.VhPattern.Angle_deg = reader.GetValue(c => c.ANTENNA.VH_PATTERN.Angle_deg);
                            stationRecord.Antenna.VhPattern.Loss_dB = reader.GetValue(c => c.ANTENNA.VH_PATTERN.Loss_dB);
                        }
                        if (reader.IsNotNull(c => c.ANTENNA.VV_PATTERN.StationId))
                        {
                            stationRecord.Antenna.VvPattern.Angle_deg = reader.GetValue(c => c.ANTENNA.VV_PATTERN.Angle_deg);
                            stationRecord.Antenna.VvPattern.Loss_dB = reader.GetValue(c => c.ANTENNA.VV_PATTERN.Loss_dB);
                        }
                        clientContextStations.Add(stationRecord);
                        var standards = clientContextStations.Select(c => c.Standard).ToArray();
                        for (int j = 0; j < standards.Length; j++)
                        {
                            var fndStations = clientContextStations.FindAll(x => x.Standard == standards[j]);
                            if (fndStations.Count > _appServerComponentConfig.MaxCountStationsByOneStandard)
                            {
                                throw new InvalidOperationException($"Too much station. For standard #{standards[j]} greater {_appServerComponentConfig.MaxCountStationsByOneStandard}. Please select other contour!");
                            }
                        }
                    }
                    return true;
                });
            }
            this._contextStations = clientContextStations.ToArray();


            // load drive tests
            var driveTests = new List<DriveTestsResult>();
            var partResultIds = BreakDownElemBlocks.BreakDown(this._parameters.InfocMeasResults);
            for (int i = 0; i < partResultIds.Count; i++)
            {

                var queryDriveTestPoints = _infocenterDataLayer.GetBuilder<IDriveTestPoints>()
                    .From()
                    .Select(
                        c => c.Id,
                        c => c.DRIVE_TEST.Freq_MHz,
                        c => c.DRIVE_TEST.Gsid,
                        c => c.DRIVE_TEST.PointsCount,
                        c => c.DRIVE_TEST.Standard,
                        c => c.DRIVE_TEST.RESULT.MeasTime,
                        c => c.Data,
                        c => c.Count
                    )
                    .Where(c => c.DRIVE_TEST.RESULT.Id, ConditionOperator.In, partResultIds[i].ToArray());

                var contextDriveTestsResults = _calcDbScope.Executor.ExecuteAndFetch(queryDriveTestPoints, reader =>
                {
                    while (reader.Read())
                    {
                        var points = reader.GetValue(c => c.Data).Deserialize<DriveTestPoint[]>();
                        var pointFS = new PointFS[points.Length];
                        for (int j=0; j<points.Length; j++)
                        {
                            pointFS[j].Coordinate = _transformation.ConvertCoordinateToEpgs(points[j].Coordinate, _transformation.ConvertProjectionToCode(this._parameters.Projection));
                            pointFS[j].FieldStrength_dBmkVm = points[j].FieldStrength_dBmkVm;
                            pointFS[j].Height_m = points[j].Height_m;
                            pointFS[j].Level_dBm = points[j].Level_dBm;
                        }

                        driveTests.Add(new DriveTestsResult()
                        {
                            Freq_MHz = reader.GetValue(c => c.DRIVE_TEST.Freq_MHz),
                            GSID = reader.GetValue(c => c.DRIVE_TEST.Gsid),
                            Standard = reader.GetValue(c => c.DRIVE_TEST.Standard),
                            Points = pointFS
                        });

                        var standards = driveTests.Select(c => c.Standard).ToArray();
                        for (int j = 0; j < standards.Length; j++)
                        {
                            var fndDriveTests = driveTests.FindAll(x => x.Standard == standards[j]);
                            var cntPoints = 0;
                            for (int k = 0; k < fndDriveTests.Count; k++)
                            {
                                cntPoints += fndDriveTests[k].Points.Length;
                            }
                            if (cntPoints > _appServerComponentConfig.MaxCountDriveTestsByOneStandard)
                            {
                                throw new InvalidOperationException($"Too much drive tests. For standard #{standards[j]} greater {_appServerComponentConfig.MaxCountDriveTestsByOneStandard}. Please select other contour!");
                            }
                        }
                    }
                    return true;
                });
            }
            this._contextDriveTestsResult = driveTests.ToArray();
        }
    }
}
