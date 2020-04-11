using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;
using System.Collections.Generic;
using Atdi.Contracts.Sdrn.CalcServer.Internal;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	internal class ProcessJob : IJobExecutor, ITaskObserver
	{
		private readonly ITaskDispatcher _taskDispatcher;
		private readonly IClientContextService _contextService;
		private readonly MapBuilder _mapBuilder;
		private readonly AppServerComponentConfig _config;
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
		private readonly ILogger _logger;

		public ProcessJob(
			ITaskDispatcher taskDispatcher,
			IClientContextService contextService,
			MapBuilder mapBuilder,
			AppServerComponentConfig config,
			IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
			ILogger logger)
		{
			_taskDispatcher = taskDispatcher;
			_contextService = contextService;
			_mapBuilder = mapBuilder;
			_config = config;
			_calcServerDataLayer = calcServerDataLayer;
			_logger = logger;
		}

		public JobExecutionResult Execute(JobExecutionContext context)
		{
			using (var dbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>())
			{
				// опрашиваем список карт
				var nextMaps = this.GetNextProjectMaps(dbScope);
				if (nextMaps != null && nextMaps.Length > 0)
				{
					foreach (var projectMapId in nextMaps)
					{
						_mapBuilder.PrepareMap(projectMapId);
					}
				}

				// опрашиваем список контекстов
				var nextContexts = this.GetNextClientContexts(dbScope);
				if (nextContexts != null && nextContexts.Length > 0)
				{
					foreach (var clientContextId in nextContexts)
					{
						_contextService.PrepareContext(dbScope, clientContextId);
					}
				}

				// опрашиваем список задач и необходимости запуска их расчета
				var nextTaskResults = this.GetNextCalcTaskResults(dbScope);
				if (nextTaskResults != null && nextTaskResults.Length > 0)
				{
					foreach (var resultId in nextTaskResults)
					{
						_taskDispatcher.RunTask(resultId, this);
					}
				}
			}

			return JobExecutionResult.Completed;
		}

		private long[] GetNextProjectMaps(IDataLayerScope dbScope)
		{
			var mapQuery = _calcServerDataLayer.GetBuilder<IProjectMap>()
				.From()
				.Select(c => c.Id)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)ProjectMapStatusCode.Pending);

			var result = dbScope.Executor.ExecuteAndFetch(mapQuery, reader =>
			{
				var res = new List<long>();
				while (reader.Read())
				{
					res.Add(reader.GetValue(c => c.Id));
				}

				return res.ToArray();
			});
			return result;
		}

		private long[] GetNextClientContexts(IDataLayerScope dbScope)
		{
			var mapQuery = _calcServerDataLayer.GetBuilder<IClientContext>()
				.From()
				.Select(c => c.Id)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)ClientContextStatusCode.Pending);

			var result = dbScope.Executor.ExecuteAndFetch(mapQuery, reader =>
			{
				var res = new List<long>();
				while (reader.Read())
				{
					res.Add(reader.GetValue(c => c.Id));
				}

				return res.ToArray();
			});
			return result;
		}

		private long[] GetNextCalcTaskResults(IDataLayerScope dbScope)
		{
			var taskResultQuery = _calcServerDataLayer.GetBuilder<ICalcResult>()
				.From()
				.Select(c => c.Id)
				// Рассчет в ожидании запуска
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcResultStatusCode.Pending)
				// статус задач Доступная 
				.Where(c => c.TASK.StatusCode, ConditionOperator.Equal, (byte)CalcTaskStatusCode.Available)
				// контекст подготовлен
				.Where(c => c.TASK.CONTEXT.StatusCode, ConditionOperator.Equal, (byte)ClientContextStatusCode.Prepared)
				// статус проекта Доступен
				.Where(c => c.TASK.CONTEXT.PROJECT.StatusCode, ConditionOperator.Equal, (byte)ProjectStatusCode.Available);

			var result = dbScope.Executor.ExecuteAndFetch(taskResultQuery, reader =>
			{
				var res = new List<long>();
				while (reader.Read())
				{
					res.Add(reader.GetValue(c => c.Id));
				}

				return res.ToArray();
			});
			return result;
		}

		public void OnCompleted(ICalcContextHandle context)
		{
			//throw new System.NotImplementedException();
		}

		public void OnEvent(ICalcContextHandle context, ICalcEvent @event)
		{
			//throw new System.NotImplementedException();
		}
	}
}
