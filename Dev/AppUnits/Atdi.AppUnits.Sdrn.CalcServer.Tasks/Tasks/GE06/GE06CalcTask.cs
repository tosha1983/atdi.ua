﻿using Atdi.Contracts.Sdrn.CalcServer;
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
        private Gn06TaskParameters _parameters;
       

      
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
            var iterationGn06CalcData = new Gn06CalcData
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
            var iterationResultGn06 = _iterationsPool.GetIteration<Gn06CalcData, Gn06CalcResult[]>();
            var resulCalibration = iterationResultGn06.Run(_taskContext, iterationGn06CalcData);


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
                return new Gn06TaskParameters()
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
    }
}