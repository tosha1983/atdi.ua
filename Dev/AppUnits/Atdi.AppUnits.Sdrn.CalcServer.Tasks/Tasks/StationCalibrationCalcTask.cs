using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.Infocenter;
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
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.Platform;
using Atdi.Common;
using Atdi.Common.Extensions;
using Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations;
using Atdi.Platform.Data;

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
        private readonly IObjectPoolSite _poolSite;
        private readonly AppServerComponentConfig _appServerComponentConfig;
        private ITaskContext _taskContext;
		private IDataLayerScope _calcDbScope;
        private IDataLayerScope _infoDbScope;
        private TaskParameters _parameters;
		private ContextStation[] _contextStations;
        private DriveTestsResult[] _contextDriveTestsResult;
       

        private class TaskParameters
		{
            public string Standard;

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
            IObjectPoolSite poolSite,
            AppServerComponentConfig appServerComponentConfig,
            IDataLayer<EntityDataOrm<IC.InfocenterEntityOrmContext>> infocenterDataLayer,
            ILogger logger)
		{
			_calcServerDataLayer = calcServerDataLayer;
            _infocenterDataLayer = infocenterDataLayer;
            _contextService = contextService;
			_mapRepository = mapRepository;
			_iterationsPool = iterationsPool;
			_transformation = transformation;
            _appServerComponentConfig = appServerComponentConfig;
            _poolSite = poolSite;
            _logger = logger;
		}

		public void Dispose()
		{
			if (_calcDbScope != null)
			{
				_calcDbScope.Dispose();
				_calcDbScope = null;
			}
            if (_infoDbScope != null)
            {
                _infoDbScope.Dispose();
                _infoDbScope = null;
            }
            _taskContext = null;
		}

        public void Load(ITaskContext taskContext)
        {
            this._taskContext = taskContext;
            this._calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>();
            this._infoDbScope = this._infocenterDataLayer.CreateScope<InfocenterDataContext>();

            // загрузить параметры задачи
            this.LoadTaskParameters();
            this.ValidateTaskParameters();
        }


        public void Run()
        {
            var mapData = _mapRepository.GetMapByName(this._calcDbScope, this._taskContext.ProjectId, this._parameters.MapName);
            var propagationModel = _contextService.GetPropagationModel(this._calcDbScope, this._taskContext.ClientContextId);
            var iterationAllStationCorellationCalcData = new AllStationCorellationCalcData
            {
                GSIDGroupeStation = this._contextStations,
                CalibrationParameters = this._parameters.CalibrationParameters,
                CorellationParameters = this._parameters.CorellationParameters,
                GSIDGroupeDriveTests = this._contextDriveTestsResult,
                GeneralParameters = this._parameters.GeneralParameters,
                MapData = mapData,
                CluttersDesc = _mapRepository.GetCluttersDesc(this._calcDbScope, mapData.Id),
                PropagationModel = propagationModel,
                Projection = this._parameters.Projection,
                FieldStrengthCalcData = new FieldStrengthCalcData
                {
                    PropagationModel = propagationModel,
                    MapArea = mapData.Area
                }
            };
            iterationAllStationCorellationCalcData.resultId = CreateResult();
            var iterationResultCalibration = _iterationsPool.GetIteration<AllStationCorellationCalcData, CalibrationResult[]>();
            var resulCalibration = iterationResultCalibration.Run(_taskContext, iterationAllStationCorellationCalcData);
            for (int i = 0; i < resulCalibration.Length; i++)
            {
                SaveTaskResult(resulCalibration[i], iterationAllStationCorellationCalcData.resultId);
            }
            // переводим результат в статус "Completed"
            var updQuery = _calcServerDataLayer.GetBuilder<ICalcResult>()
               .Update()
               .SetValue(c => c.StatusCode, (byte)CalcResultStatusCode.Completed)
               .SetValue(c => c.StatusName, CalcResultStatusCode.Completed.ToString())
               .SetValue(c => c.StatusNote, "The calc  result completed")
               .Where(c => c.TASK.Id, ConditionOperator.Equal, _taskContext.TaskId)
               .Where(c => c.Id, ConditionOperator.Equal, _taskContext.ResultId);
            _calcDbScope.Executor.Execute(updQuery);

            //CalibrationResult resulCalibration1 = new CalibrationResult()
            //{

            //    AreaName = "Area",
            //    CountMeasGSID = 1,
            //    IdResult = 67,
            //    GeneralParameters = new GeneralParameters()
            //    {
            //        DistanceAroundContour_km = 22,
            //        TrustOldResults = true
            //    },
            //    ResultCalibrationDriveTest = new CalibrationDriveTestResult[2]
            //              {
            //                   new CalibrationDriveTestResult()
            //                   {
            //                         DriveTestId = 1,
            //                        CountPointsInDriveTest=1,
            //                         Gsid="22",
            //                          ResultDriveTestStatus = DriveTestStatusResult.LS,
            //                           LinkToStationMonitoringId = 4325
            //                   },
            //                   new CalibrationDriveTestResult()
            //                   {
            //                         DriveTestId = 1,
            //                        CountPointsInDriveTest=1,
            //                         Gsid="23",
            //                          ResultDriveTestStatus = DriveTestStatusResult.UN,
            //                           LinkToStationMonitoringId = 4325
            //                   }
            //              },
            //    ResultCalibrationStation = new CalibrationStationResult[2]
            //               {
            //                    new CalibrationStationResult()
            //                    {
            //                         StationMonitoringId = 4325,
            //                         ExternalCode="2",
            //                          ExternalSource = "MOB_STATION",
            //                           RealGsid="RG2",
            //                            ResultStationStatus = StationStatusResult.CS,


            //                    },
            //                    new CalibrationStationResult()
            //                    {
            //                         StationMonitoringId = 4325,
            //                         ExternalCode="3",
            //                          ExternalSource = "MOB_STATION",
            //                           RealGsid="RG1",
            //                            ResultStationStatus = StationStatusResult.NF,


            //                    }
            //               }

            //};
            //SaveTaskResult(in resulCalibration1);
            //SaveTaskResult(in resulCalibration1);
            //SaveTaskResult(in resulCalibration1);
        }

        private void ValidateTaskParameters()
		{
            if (this._parameters != null)
            {
                // General parameter
                if (((this._parameters.GeneralParameters.СorrelationThresholdHard >= 0) && (this._parameters.GeneralParameters.СorrelationThresholdHard <= 100)) == false)
                {
                    throw new InvalidOperationException("General parameter 'СorrelationThresholdHard' incorrect");
                }
                if (((this._parameters.GeneralParameters.СorrelationThresholdWeak >= 0) && (this._parameters.GeneralParameters.СorrelationThresholdWeak <= 100)) == false)
                {
                    throw new InvalidOperationException("General parameter 'СorrelationThresholdWeak' incorrect");
                }
                if (((this._parameters.GeneralParameters.DistanceAroundContour_km >= 0) && (this._parameters.GeneralParameters.DistanceAroundContour_km <= 50)) == false)
                {
                    throw new InvalidOperationException("General parameter 'DistanceAroundContour_km' incorrect");
                }
                if (((this._parameters.GeneralParameters.MinNumberPointForCorrelation >= 1) && (this._parameters.GeneralParameters.MinNumberPointForCorrelation <= 1000)) == false)
                {
                    throw new InvalidOperationException("General parameter 'MinNumberPointForCorrelation' incorrect");
                }

                // Correlation parameter
                if (((this._parameters.CorellationParameters.MinRangeMeasurements_dBmkV >= -200) && (this._parameters.CorellationParameters.MinRangeMeasurements_dBmkV <= 200)) == false)
                {
                    throw new InvalidOperationException("Correlation  parameter 'MinRangeMeasurements_dBmkV' incorrect");
                }
                if (((this._parameters.CorellationParameters.MaxRangeMeasurements_dBmkV >= -200) && (this._parameters.CorellationParameters.MaxRangeMeasurements_dBmkV <= 200)) == false)
                {
                    throw new InvalidOperationException("Correlation  parameter 'MaxRangeMeasurements_dBmkV' incorrect");
                }
                if (((this._parameters.CorellationParameters.CorrelationDistance_m >= 0) && (this._parameters.CorellationParameters.CorrelationDistance_m <= 100)) == false)
                {
                    throw new InvalidOperationException("Correlation  parameter 'CorrelationDistance_m' incorrect");
                }
                if (((this._parameters.CorellationParameters.Delta_dB >= 0) && (this._parameters.CorellationParameters.Delta_dB <= 20)) == false)
                {
                    throw new InvalidOperationException("Correlation  parameter 'Delta_dB' incorrect");
                }
                if (((this._parameters.CorellationParameters.MaxAntennasPatternLoss_dB >= 0) && (this._parameters.CorellationParameters.MaxAntennasPatternLoss_dB <= 200)) == false)
                {
                    throw new InvalidOperationException("Correlation  parameter 'MaxAntennasPatternLoss_dB' incorrect");
                }

                // Calibration parameter
                if (((this._parameters.CalibrationParameters.ShiftAltitudeStationMin_m >= -100) && (this._parameters.CalibrationParameters.ShiftAltitudeStationMin_m <= 100)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'ShiftAltitudeStationMin_m' incorrect");
                }
                if (((this._parameters.CalibrationParameters.ShiftAltitudeStationMax_m >= -100) && (this._parameters.CalibrationParameters.ShiftAltitudeStationMax_m <= 100)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'ShiftAltitudeStationMax_m' incorrect");
                }
                if (((this._parameters.CalibrationParameters.ShiftAltitudeStationStep_m >= 1) && (this._parameters.CalibrationParameters.ShiftAltitudeStationStep_m <= 10)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'ShiftAltitudeStationStep_m' incorrect");
                }
                if (((this._parameters.CalibrationParameters.MaxDeviationAltitudeStation_m >= 0) && (this._parameters.CalibrationParameters.MaxDeviationAltitudeStation_m <= 100)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'MaxDeviationAltitudeStation_m' incorrect");
                }
                if (((this._parameters.CalibrationParameters.ShiftTiltStationMin_Deg >= -100) && (this._parameters.CalibrationParameters.ShiftTiltStationMin_Deg <= 100)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'ShiftTiltStationMin_Deg' incorrect");
                }
                if (((this._parameters.CalibrationParameters.ShiftTiltStationMax_Deg >= -100) && (this._parameters.CalibrationParameters.ShiftTiltStationMax_Deg <= 100)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'ShiftTiltStationMax_Deg' incorrect");
                }
                if (((this._parameters.CalibrationParameters.ShiftTiltStationStep_Deg >= 1) && (this._parameters.CalibrationParameters.ShiftTiltStationStep_Deg <= 10)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'ShiftTiltStationStep_Deg' incorrect");
                }
                if (((this._parameters.CalibrationParameters.MaxDeviationTiltStationDeg >= 0) && (this._parameters.CalibrationParameters.MaxDeviationTiltStationDeg <= 100)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'MaxDeviationTiltStationDeg' incorrect");
                }
                if (((this._parameters.CalibrationParameters.ShiftAzimuthStationMin_deg >= -200) && (this._parameters.CalibrationParameters.ShiftAzimuthStationMin_deg <= 200)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'ShiftAzimuthStationMin_deg' incorrect");
                }
                if (((this._parameters.CalibrationParameters.ShiftAzimuthStationMax_deg >= -200) && (this._parameters.CalibrationParameters.ShiftAzimuthStationMax_deg <= 200)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'ShiftAzimuthStationMax_deg' incorrect");
                }
                if (((this._parameters.CalibrationParameters.ShiftAzimuthStationStep_deg >= 1) && (this._parameters.CalibrationParameters.ShiftAzimuthStationStep_deg <= 10)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'ShiftAzimuthStationStep_deg' incorrect");
                }
                if (((this._parameters.CalibrationParameters.MaxDeviationAzimuthStation_deg >= 0) && (this._parameters.CalibrationParameters.MaxDeviationAzimuthStation_deg <= 200)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'MaxDeviationAzimuthStation_deg' incorrect");
                }
                if (((this._parameters.CalibrationParameters.ShiftCoordinatesStation_m >= 0) && (this._parameters.CalibrationParameters.ShiftCoordinatesStation_m <= 1000)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'ShiftCoordinatesStation_m' incorrect");
                }
                if (((this._parameters.CalibrationParameters.ShiftCoordinatesStationStep_m >= 1) && (this._parameters.CalibrationParameters.ShiftCoordinatesStationStep_m <= 100)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'ShiftCoordinatesStationStep_m' incorrect");
                }
                if (((this._parameters.CalibrationParameters.MaxDeviationCoordinatesStation_m >= 0) && (this._parameters.CalibrationParameters.MaxDeviationCoordinatesStation_m <= 1000)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'MaxDeviationCoordinatesStation_m' incorrect");
                }
                if (((this._parameters.CalibrationParameters.ShiftPowerStationMin_dB >= -50) && (this._parameters.CalibrationParameters.ShiftPowerStationMin_dB <= 50)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'ShiftPowerStationMin_dB' incorrect");
                }
                if (((this._parameters.CalibrationParameters.ShiftPowerStationMax_dB >= -50) && (this._parameters.CalibrationParameters.ShiftPowerStationMax_dB <= 50)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'ShiftPowerStationMax_dB' incorrect");
                }
                if (((this._parameters.CalibrationParameters.ShiftPowerStationStep_dB >= 0) && (this._parameters.CalibrationParameters.ShiftPowerStationStep_dB <= 5)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'ShiftPowerStationStep_dB' incorrect");
                }
                if (((this._parameters.CalibrationParameters.NumberCascade >= 1) && (this._parameters.CalibrationParameters.NumberCascade <= 5)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'NumberCascade' incorrect");
                }
                if (((this._parameters.CalibrationParameters.DetailOfCascade >= 2) && (this._parameters.CalibrationParameters.DetailOfCascade <= 10)) == false)
                {
                    throw new InvalidOperationException("CalibrationParameters  parameter 'DetailOfCascade' incorrect");
                }
            }
        }

        private void LoadTaskParameters()
        {
            // load parameters
            var query = _calcServerDataLayer.GetBuilder<IStationCalibrationArgs>()
                            .From()
                            .Select(
                                c => c.Standard,
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
                                c => c.CorrelationThresholdHard,
                                c => c.CorrelationThresholdWeak,
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
                        TrustOldResults = reader.GetValue(c => c.TrustOldResults),
                        UseMeasurementSameGSID = reader.GetValue(c => c.UseMeasurementSameGSID),
                        СorrelationThresholdHard = reader.GetValue(c => c.CorrelationThresholdHard).GetValueOrDefault(),
                        СorrelationThresholdWeak = reader.GetValue(c => c.CorrelationThresholdWeak).GetValueOrDefault()
                    },
                    CorellationParameters = new CorellationParameters()
                    {
                        CorrelationDistance_m = reader.GetValue(c => c.CorrelationDistance_m).GetValueOrDefault(),
                        Delta_dB = reader.GetValue(c => c.Delta_dB).GetValueOrDefault(),
                        Detail = reader.GetValue(c => c.Detail),
                        MaxAntennasPatternLoss_dB = reader.GetValue(c => c.MaxAntennasPatternLoss_dB).GetValueOrDefault(),
                        MaxRangeMeasurements_dBmkV = reader.GetValue(c => c.MaxRangeMeasurements_dBmkV).GetValueOrDefault(),
                        MinRangeMeasurements_dBmkV = reader.GetValue(c => c.MinRangeMeasurements_dBmkV).GetValueOrDefault()
                    },
                    CalibrationParameters = new CalibrationParameters()
                    {
                        AltitudeStation = reader.GetValue(c => c.AltitudeStation),
                        AzimuthStation = reader.GetValue(c => c.AzimuthStation),
                        CascadeTuning = reader.GetValue(c => c.CascadeTuning),
                        CoordinatesStation = reader.GetValue(c => c.CoordinatesStation),
                        DetailOfCascade = reader.GetValue(c => c.DetailOfCascade).GetValueOrDefault(),
                        MaxDeviationAltitudeStation_m = reader.GetValue(c => c.MaxDeviationAltitudeStation_m).GetValueOrDefault(),
                        MaxDeviationAzimuthStation_deg = reader.GetValue(c => c.MaxDeviationAzimuthStation_deg).GetValueOrDefault(),
                        MaxDeviationCoordinatesStation_m = reader.GetValue(c => c.MaxDeviationCoordinatesStation_m).GetValueOrDefault(),
                        MaxDeviationTiltStationDeg = reader.GetValue(c => c.MaxDeviationTiltStation_deg).GetValueOrDefault(),
                        Method = method,
                        NumberCascade = reader.GetValue(c => c.NumberCascade).GetValueOrDefault(),
                        PowerStation = reader.GetValue(c => c.PowerStation),
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
                        TiltStation = reader.GetValue(c => c.TiltStation)
                    },
                    Projection = reader.GetValue(c => c.TASK.CONTEXT.PROJECT.Projection),
                    MapName = reader.GetValue(c => c.TASK.MapName),
                    StationIds = reader.GetValue(c => c.StationIds),
                    InfocMeasResults = reader.GetValue(c => c.InfocMeasResults),
                    Standard = reader.GetValue(c => c.Standard)
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
                        c => c.ExternalCode,
                        c => c.ExternalSource,
                        c => c.CONTEXT.Id,
                        c => c.StateCode,
                        c => c.CreatedDate,
                        c => c.CallSign,
                        c => c.Name,
                        c => c.Standard,
                        c => c.ModifiedDate,

                        c => c.LicenseGsid,
                        c => c.RealGsid,
                        c => c.RegionCode,


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
                            Standard = reader.GetValue(c => c.Standard).GetStandardForDriveTest(),
                            RealStandard = reader.GetValue(c => c.Standard),
                            ExternalCode = reader.GetValue(c => c.ExternalCode),
                            ExternalSource = reader.GetValue(c => c.ExternalSource),
                            ModifiedDate = reader.GetValue(c => c.ModifiedDate),
                            LicenseGsid = reader.GetValue(c => c.LicenseGsid),
                            RealGsid = reader.GetValue(c => c.RealGsid),
                            RegionCode = reader.GetValue(c => c.RegionCode),

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

                        //var arrStations = DuplicateContextStation(stationRecord, reader.GetValue(c => c.Standard));
                        //if ((arrStations != null) && (arrStations.Length > 0))
                        //{
                        //    clientContextStations.AddRange(arrStations);
                        //}
                        //else
                        //{
                            clientContextStations.Add(stationRecord);
                        //}
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
            int cntRecord = 0;
            var allDriveTests = new List<DriveTestsResult>();
            var partResultIds = BreakDownElemBlocks.BreakDown(this._parameters.InfocMeasResults);
            for (int i = 0; i < partResultIds.Count; i++)
            {
                var driveTests = new List<DriveTestsResult>();
                var queryDriveTest = _infocenterDataLayer.GetBuilder<IDriveTest>()
                .From()
                .Select(
                    c => c.Id,
                    c => c.Freq_MHz,
                    c => c.Gsid,
                    c => c.PointsCount,
                    c => c.Standard,
                    c => c.RESULT.MeasTime
                )
                .Where(c => c.RESULT.Id, ConditionOperator.In, partResultIds[i].ToArray())
                .Where(c => c.Standard, ConditionOperator.Equal, this._parameters.Standard.GetStandardForDriveTest());
                var contextDriveTestsResults = _infoDbScope.Executor.ExecuteAndFetch(queryDriveTest, reader =>
                {
                    while (reader.Read())
                    {

                        var driveTestsResult = new DriveTestsResult()
                        {
                            DriveTestId = reader.GetValue(c => c.Id),
                            Freq_MHz = reader.GetValue(c => c.Freq_MHz),
                            GSID = reader.GetValue(c => c.Gsid),
                            Standard = reader.GetValue(c => c.Standard).GetStandardForDriveTest(),
                            RealStandard = reader.GetValue(c => c.Standard),
                            Num = reader.GetValue(c => c.Id)
                        };

                        driveTests.Add(driveTestsResult);


                        cntRecord++;
                    }
                    return true;
                });

                var allListNumsDriveTests = driveTests.Select(b => b.Num);
                if ((allListNumsDriveTests != null) && (allListNumsDriveTests.Count() > 0))
                {
                    var breakDownListNumsDrivePoints = BreakDownElemBlocks.BreakDown(allListNumsDriveTests.ToArray());
                    for (int k = 0; k < breakDownListNumsDrivePoints.Count(); k++)
                    {

                        var queryDriveTestPoints = _infocenterDataLayer.GetBuilder<IDriveTestPoints>()
                        .From()
                        .Select(
                        c => c.Id,
                        c => c.DRIVE_TEST.Id,
                        c => c.Points,
                        c => c.Count
                        )
                        .Where(c => c.DRIVE_TEST.Id, ConditionOperator.In, breakDownListNumsDrivePoints[k] /*driveTest.Num*/);

                        var contextDriveTestsPointResults = _infoDbScope.Executor.ExecuteAndFetch(queryDriveTestPoints, DriveTestsPoint =>
                        {
                            while (DriveTestsPoint.Read())
                            {
                                var drvTestId = DriveTestsPoint.GetValue(c => c.DRIVE_TEST.Id);

                                var fndDrvTest = driveTests.Find(n => n.DriveTestId == drvTestId);
                                if (fndDrvTest != null)
                                {
                                    var lstDrvPOints = new List<PointFS>();
                                    if ((fndDrvTest.Points != null) && (fndDrvTest.Points.Length > 0))
                                    {
                                        lstDrvPOints = fndDrvTest.Points.ToList();
                                    }
                                    PointFS[] pointFS = null;
                                    var points = DriveTestsPoint.GetValueAs<DriveTestPoint[]>(c => c.Points);
                                    if (points != null)
                                    {
                                        pointFS = new PointFS[points.Length];
                                        for (int j = 0; j < points.Length; j++)
                                        {
                                            pointFS[j].Coordinate = _transformation.ConvertCoordinateToEpgs(points[j].Coordinate, _transformation.ConvertProjectionToCode(this._parameters.Projection));
                                            pointFS[j].FieldStrength_dBmkVm = points[j].FieldStrength_dBmkVm;
                                            //if (points[j].Height_m == 0)
                                            //{
                                            pointFS[j].Height_m = 3;
                                            //}
                                            //else
                                            //{
                                            //    pointFS[j].Height_m = points[j].Height_m;
                                            //}
                                            pointFS[j].Level_dBm = points[j].Level_dBm;
                                        }
                                        lstDrvPOints.AddRange(pointFS);
                                        fndDrvTest.CountPoints = lstDrvPOints.Count();
                                        fndDrvTest.Points = lstDrvPOints.ToArray();
                                    }
                                }
                            }
                            return true;
                        });
                    }
                }
                allDriveTests.AddRange(driveTests);
            }
            this._contextDriveTestsResult = allDriveTests.ToArray();
        }

        private long CreateResult()
        {
            var insertQueryStationCalibrationResult = _calcServerDataLayer.GetBuilder<IStationCalibrationResult>()
                .Insert()
                .SetValue(c => c.TimeStart, DateTime.Now)
                .SetValue(c => c.PARAMETERS.TaskId, _taskContext.TaskId)
                .SetValue(c => c.RESULT.Id, _taskContext.ResultId)
                ;
            var key = _calcDbScope.Executor.Execute<IStationCalibrationResult_PK>(insertQueryStationCalibrationResult);
            return key.Id;
        }
        private void UpdatePercentComplete(long resultId, int percentComplete)
        {
            var updateQueryStationCalibrationResult = _calcServerDataLayer.GetBuilder<IStationCalibrationResult>()
                          .Update()
                          .SetValue(c => c.PercentComplete, percentComplete)
                          .Where(c => c.Id, ConditionOperator.Equal, resultId);
            _calcDbScope.Executor.Execute(updateQueryStationCalibrationResult);
        }

        //private ContextStation[] DuplicateContextStation(ContextStation contextStation, string standard)
        //{
        //    var lstDuplicateStations = new List<ContextStation>();
        //    if (this._appServerComponentConfig.StandardsForDuplicateStations != null)
        //    {
        //        if (this._appServerComponentConfig.StandardsForDuplicateStations.Contains(standard))
        //        {
        //            if (contextStation != null)
        //            {
        //                if ((contextStation.Transmitter.Freqs_MHz != null) && (contextStation.Receiver.Freqs_MHz != null))
        //                {
        //                    if ((contextStation.Transmitter.Freqs_MHz.Length > 0) && (contextStation.Receiver.Freqs_MHz.Length > 0))
        //                    {
        //                        for (int p = 0; p < contextStation.Transmitter.Freqs_MHz.Length; p++)
        //                        {
        //                            ContextStation copystation = Atdi.Common.CopyHelper.CreateDeepCopy(contextStation);
        //                            copystation.Transmitter.Freq_MHz = contextStation.Transmitter.Freqs_MHz[p];
        //                            copystation.Receiver.Freq_MHz = contextStation.Receiver.Freqs_MHz[p];

        //                            lstDuplicateStations.Add(copystation);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return lstDuplicateStations.ToArray();
        //}

        private void SaveTaskResult(in CalibrationResult result, long resultId)
        {
            var updateQueryStationCalibrationResult = _calcServerDataLayer.GetBuilder<IStationCalibrationResult>()
                .Update()
                .SetValue(c => c.AreaName, result.AreaName)
                .SetValue(c => c.CountMeasGSID, result.CountMeasGSID)
                .SetValue(c => c.CountMeasGSID_IT, result.CountMeasGSID_IT)
                .SetValue(c => c.CountMeasGSID_LS, result.CountMeasGSID_LS)
                .SetValue(c => c.CountStation_CS, result.CountStation_CS)
                .SetValue(c => c.CountStation_IT, result.CountStation_IT)
                .SetValue(c => c.CountStation_NF, result.CountStation_NF)
                .SetValue(c => c.CountStation_NS, result.CountStation_NS)
                .SetValue(c => c.CountStation_UN, result.CountStation_UN)
                .SetValue(c => c.NumberStation, result.NumberStation)
                .SetValue(c => c.NumberStationInContour, result.NumberStationInContour)
                .SetValue(c => c.Standard, result.Standard)
                .Where(c => c.Id, ConditionOperator.Equal, resultId)
                ;

            if (_calcDbScope.Executor.Execute(updateQueryStationCalibrationResult)>0)
            {
                for (int z = 0; z < result.ResultCalibrationDriveTest.Length; z++)
                {
                    var driveTest = result.ResultCalibrationDriveTest[z];
                    var insertQueryStationCalibrationDriveTestResult = _calcServerDataLayer.GetBuilder<IStationCalibrationDriveTestResult>()
                    .Insert()
                    .SetValue(c => c.CalibrationResultId, resultId)
                    .SetValue(c => c.CountPointsInDriveTest, driveTest.CountPointsInDriveTest)
                    .SetValue(c => c.ExternalCode, driveTest.ExternalCode)
                    .SetValue(c => c.ExternalSource, driveTest.ExternalSource)
                    .SetValue(c => c.MeasGcid, driveTest.Gsid)
                    .SetValue(c => c.StationGcid, driveTest.GsidFromStation)
                    .SetValue(c => c.DriveTestId, driveTest.DriveTestId)
                    .SetValue(c => c.MaxPercentCorellation, driveTest.MaxPercentCorellation)
                    .SetValue(c => c.LinkToStationMonitoringId, driveTest.LinkToStationMonitoringId)
                    .SetValue(c => c.ResultDriveTestStatus, driveTest.ResultDriveTestStatus.ToString())
                    .SetValue(c => c.Freq_MHz, driveTest.Freq_MHz)
                    .SetValue(c => c.Standard, driveTest.Standard)
                    ;
                    _calcDbScope.Executor.Execute<IStationCalibrationDriveTestResult_PK>(insertQueryStationCalibrationDriveTestResult);
                }

                for (int z = 0; z < result.ResultCalibrationStation.Length; z++)
                {
                    var station = result.ResultCalibrationStation[z];
                    var insertQueryStationCalibrationStaResult = _calcServerDataLayer.GetBuilder<IStationCalibrationStaResult>()
                    .Insert()
                    .SetValue(c => c.StationMonitoringId, station.StationMonitoringId)
                    .SetValue(c => c.CalibrationResultId, resultId)
                    .SetValue(c => c.ExternalCode, station.ExternalCode)
                    .SetValue(c => c.ExternalSource, station.ExternalSource)
                    .SetValue(c => c.LicenseGsid, station.LicenseGsid)
                    .SetValue(c => c.MaxCorellation, station.MaxCorellation);
                    if (station.ParametersStationNew != null)
                    {
                        insertQueryStationCalibrationStaResult.SetValue(c => c.New_Altitude_m, station.ParametersStationNew.Altitude_m)
                       .SetValue(c => c.New_Azimuth_deg, station.ParametersStationNew.Azimuth_deg)
                       .SetValue(c => c.New_Lat_deg, station.ParametersStationNew.Lat_deg)
                       .SetValue(c => c.New_Lon_deg, station.ParametersStationNew.Lon_deg)
                       .SetValue(c => c.New_Power_dB, station.ParametersStationNew.Power_dB)
                       .SetValue(c => c.New_Tilt_deg, station.ParametersStationNew.Tilt_Deg);
                    }
                    if (station.ParametersStationOld != null)
                    {
                        insertQueryStationCalibrationStaResult.SetValue(c => c.Old_Altitude_m, station.ParametersStationOld.Altitude_m)
                        .SetValue(c => c.Old_Azimuth_deg, station.ParametersStationOld.Azimuth_deg)
                        .SetValue(c => c.Old_Freq_MHz, station.ParametersStationOld.Freq_MHz)
                        .SetValue(c => c.Old_Lat_deg, station.ParametersStationOld.Lat_deg)
                        .SetValue(c => c.Old_Lon_deg, station.ParametersStationOld.Lon_deg)
                        .SetValue(c => c.Old_Power_dB, station.ParametersStationOld.Power_dB)
                        .SetValue(c => c.Old_Tilt_deg, station.ParametersStationOld.Tilt_Deg);
                    }
                    insertQueryStationCalibrationStaResult.SetValue(c => c.RealGsid, station.RealGsid)
                    .SetValue(c => c.ResultStationStatus, station.ResultStationStatus.ToString())
                    .SetValue(c => c.Freq_MHz, station.Freq_MHz)
                    .SetValue(c => c.Standard, station.Standard);
                    _calcDbScope.Executor.Execute<IStationCalibrationStaResult_PK>(insertQueryStationCalibrationStaResult);
                }
            }
        }
    }
}
