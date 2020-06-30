using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.Infocenter;
using System;
using System.Collections.Generic;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.CalcServer.Internal;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Logging;
using Atdi.DataModels.DataConstraint;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using CALC = Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.Platform.Data;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks
{
	[TaskHandler(CalcTaskType.Gn06CalcTask)]
	public class GE06CalcTask : ITaskHandler
	{
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
        private readonly IClientContextService _contextService;
		private readonly IMapRepository _mapRepository;
		private readonly IIterationsPool _iterationsPool;
        private readonly ITransformation _transformation;
		private readonly ILogger _logger;
        private readonly IObjectPoolSite _poolSite;
        private readonly AppServerComponentConfig _appServerComponentConfig;
        private ITaskContext _taskContext;
		private IDataLayerScope _calcDbScope;
        private Ge06TaskParameters _parameters;
       

      
		public GE06CalcTask(
			IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
			IClientContextService contextService,
			IMapRepository mapRepository,
			IIterationsPool iterationsPool,
			ITransformation transformation,
            IObjectPoolSite poolSite,
            AppServerComponentConfig appServerComponentConfig,
            ILogger logger)
		{
			_calcServerDataLayer = calcServerDataLayer;
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
            _taskContext = null;
		}

        public void Load(ITaskContext taskContext)
        {
            this._taskContext = taskContext;
            this._calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>();
            // загрузить параметры задачи
            this.LoadTaskParameters();
        }


        public void Run()
        {
            var mapData = _mapRepository.GetMapByName(this._calcDbScope, this._taskContext.ProjectId, this._parameters.MapName);
            var propagationModel = _contextService.GetPropagationModel(this._calcDbScope, this._taskContext.ClientContextId);
            var iterationGe06CalcData = new Ge06CalcData
            {
                Gn06TaskParameters = this._parameters,
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
            var resultId = CreateGe06Result();
            var iterationResultGe06 = _iterationsPool.GetIteration<Ge06CalcData, Ge06CalcResult[]>();
            var resultGe06Calc = iterationResultGe06.Run(_taskContext, iterationGe06CalcData);
            for (int i = 0; i < resultGe06Calc.Length; i++)
            {
                SaveTaskResult(resultGe06Calc[i], resultId);
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
        }
       
        private void LoadTaskParameters()
        {
            // load parameters
            var query = _calcServerDataLayer.GetBuilder<CALC.IGn06Args>()
                            .From()
                            .Select(
                                c => c.AdditionalContoursByDistances,
                                c => c.AzimuthStep_deg,
                                c => c.BroadcastingExtend,
                                c => c.CalculationTypeCode,
                                c => c.CalculationTypeName,
                                c => c.ContureByFieldStrength,
                                c => c.Distances,
                                c => c.FieldStrength,
                                c => c.PercentageTime,
                                c => c.SubscribersHeight,
                                c => c.UseEffectiveHeight,
                                c => c.BroadcastingExtend,
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
                return new Ge06TaskParameters()
                {
                    CalculationTypeCode = reader.GetValue(c => c.CalculationTypeCode),
                    CalculationTypeName = reader.GetValue(c => c.CalculationTypeName),
                    AdditionalContoursByDistances = reader.GetValue(c => c.AdditionalContoursByDistances),
                    AzimuthStep_deg = reader.GetValue(c => c.AzimuthStep_deg),
                    BroadcastingContext = reader.GetValueAs<BroadcastingContext>(c => c.BroadcastingExtend),
                    UseEffectiveHeight = reader.GetValue(c => c.UseEffectiveHeight),
                    SubscribersHeight = reader.GetValue(c => c.SubscribersHeight),
                    PercentageTime = reader.GetValue(c => c.PercentageTime),
                    ContureByFieldStrength = reader.GetValue(c => c.ContureByFieldStrength),
                    FieldStrength = reader.GetValue(c => c.FieldStrength),
                    Distances = reader.GetValue(c => c.Distances),
                    Projection = reader.GetValue(c => c.TASK.CONTEXT.PROJECT.Projection),
                    MapName = reader.GetValue(c => c.TASK.MapName),

                };
            });
        }

        private long CreateGe06Result()
        {
            var insertQueryGn06Result = _calcServerDataLayer.GetBuilder<CALC.IGn06Result>()
                .Insert()
                .SetValue(c => c.RESULT.Id, _taskContext.ResultId);
            var key = _calcDbScope.Executor.Execute<CALC.IGn06Result_PK>(insertQueryGn06Result);
            return key.Id;
        }

        private void SaveTaskResult(in Ge06CalcResult result, long gn06ResultId)
        {
            if (result.ContoursResult != null)
            {
                var contoursResult = result.ContoursResult;
                var insertQueryGn06ContoursResult = _calcServerDataLayer.GetBuilder<CALC.IGn06ContoursResult>()
                    .Insert()
                    .SetValue(c => c.AffectedADM, contoursResult.AffectedADM)
                    .SetValue(c => c.ContourType, (byte)contoursResult.ContourType)
                    .SetValueAsJson<CountoursPoint[]>(c => c.CountoursPoints, contoursResult.CountoursPoints)
                    .SetValue(c => c.Distance, contoursResult.Distance)
                    .SetValue(c => c.FS, contoursResult.FS)
                    .SetValue(c => c.PointsCount, contoursResult.PointsCount)
                    .SetValue(c => c.Gn06ResultId, gn06ResultId);
                var keyGn06ContoursResult = _calcDbScope.Executor.Execute<CALC.IGn06ContoursResult_PK>(insertQueryGn06ContoursResult);
            }
            if (result.AffectedADMResult != null)
            {
                var affectedADMResult = result.AffectedADMResult;
                var insertQueryGn06AffectedADMResult = _calcServerDataLayer.GetBuilder<CALC.IGn06AffectedADMResult>()
                    .Insert()
                    .SetValue(c => c.ADM, affectedADMResult.ADM)
                    .SetValue(c => c.AffectedServices, affectedADMResult.AffectedServices)
                    .SetValue(c => c.Gn06ResultId, gn06ResultId)
                    .SetValue(c => c.TypeAffected, affectedADMResult.TypeAffected);
                var keyGn06AffectedADMResult = _calcDbScope.Executor.Execute<CALC.IGn06AffectedADMResult_PK>(insertQueryGn06AffectedADMResult);
            }
        }
        }
    }
}
