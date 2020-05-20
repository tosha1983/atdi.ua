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
using Atdi.Platform;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks
{
	[TaskHandler(CalcTaskType.StationCalibrationCalcTask)]
	public class StationCalibrationCalcTask : ITaskHandler
	{
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
		private readonly IClientContextService _contextService;
		private readonly IMapRepository _mapRepository;
		private readonly IIterationsPool _iterationsPool;
        private readonly ITransformation _transformation;
		private readonly ILogger _logger;
		private ITaskContext _taskContext;
		private IDataLayerScope _calcDbScope;
		private TaskParameters _parameters;
		private ClientContextStation[] _contextStations;
        private DriveTestsResult[] _contextDriveTestsResult;
        private PropagationModel _propagationModel;
        private ProjectMapData _mapData;
		private CluttersDesc _cluttersDesc;


		private class TaskParameters
		{
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
			ILogger logger)
		{
			_calcServerDataLayer = calcServerDataLayer;
			_contextService = contextService;
			_mapRepository = mapRepository;
			_iterationsPool = iterationsPool;
			_transformation = transformation;
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

            //заполнение this._contextStations (пока не знаю каким способом)

            //заполнение this._contextDriveTestsResult (пока не знаю каким способом)

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
                GeneralParameters = this._parameters.GeneralParameters,
                CodeProjection = _transformation.ConvertProjectionToCode(this._parameters.Projection)
            };

            
            

            var iterationResultCalibration = _iterationsPool.GetIteration<AllStationCorellationCalcData, ResultCalibration>();
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
            var query = _calcServerDataLayer.GetBuilder<ICalibrationParametersArgs>()
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
                                c => c.MaxDeviationTiltStationDeg,
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
                                c => c.ShiftTiltStationMax_Deg,
                                c => c.ShiftTiltStationMin_Deg,
                                c => c.ShiftTiltStationStep_Deg,
                                c => c.TiltStation,
                                c => c.TASK.CONTEXT.PROJECT.Projection,
                                c => c.TASK.MapName
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
                         MaxDeviationTiltStationDeg = reader.GetValue(c => c.MaxDeviationTiltStationDeg).GetValueOrDefault(),
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
                         ShiftTiltStationMax_Deg = reader.GetValue(c => c.ShiftTiltStationMax_Deg).GetValueOrDefault(),
                         ShiftTiltStationMin_Deg = reader.GetValue(c => c.ShiftTiltStationMin_Deg).GetValueOrDefault(),
                         ShiftTiltStationStep_Deg = reader.GetValue(c => c.ShiftTiltStationStep_Deg).GetValueOrDefault(),
                         TiltStation = reader.GetValue(c => c.TiltStation).GetValueOrDefault()
                    },
                    Projection = reader.GetValue(c => c.TASK.CONTEXT.PROJECT.Projection),
                    MapName = reader.GetValue(c => c.TASK.MapName)
                };
            });
        }
    }
}
