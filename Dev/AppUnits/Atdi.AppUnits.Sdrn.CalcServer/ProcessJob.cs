using System;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;
using System.Collections.Generic;
using Atdi.Contracts.Sdrn.CalcServer.Internal;
using Atdi.DataModels.Sdrn.CalcServer;

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
				// при первом запуске, попытка восстановить расчеты по задачам которые имеют точки восстановления - остальные нужно прервать
				if (!context.IsRepeat)
				{
					var nextRecoveryTaskResults = this.GetNextCalcTaskResults(dbScope);
					if (nextRecoveryTaskResults != null && nextRecoveryTaskResults.Length > 0)
					{
						foreach (var resultId in nextRecoveryTaskResults)
						{
							_taskDispatcher.RunTask(resultId, this);
						}
					}
				}


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

		private long[] GetNextRecoverCalcTaskResults(IDataLayerScope dbScope)
		{
			var taskResultQuery = _calcServerDataLayer.GetBuilder<ICalcResult>()
				.From()
				.Select(c => c.Id)
				// Рассчет в ожидании запуска
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcResultStatusCode.Processing)
				// статус задач Доступная 
				.Where(c => c.TASK.StatusCode, ConditionOperator.Equal, (byte)CalcTaskStatusCode.Available)
				// контекст подготовлен
				.Where(c => c.TASK.CONTEXT.StatusCode, ConditionOperator.Equal, (byte)ClientContextStatusCode.Prepared)
				// статус проекта Доступен
				.Where(c => c.TASK.CONTEXT.PROJECT.StatusCode, ConditionOperator.Equal, (byte)ProjectStatusCode.Available);

			var ids = dbScope.Executor.ExecuteAndFetch(taskResultQuery, reader =>
			{
				var res = new List<long>();
				while (reader.Read())
				{
					var resultId = reader.GetValue(c => c.Id);
					
					res.Add(resultId);
				}

				return res.ToArray();
			});

			var result = new List<long>();
			foreach (var resultId in ids)
			{
				// проверим есть ли точка востановления
				if (ExistsCheckPoint(dbScope, resultId))
				{
					result.Add(resultId);
				}
				else
				{
					this.ChangeTaskResultStatusToAborted(dbScope, resultId, "The calculation task was aborted by restarting the application server");
				}

			}

			return result.ToArray();
		}
		private bool ChangeTaskResultStatusToAborted(IDataLayerScope calcDbScope, long resultId, string reason)
		{
			var query = _calcServerDataLayer.GetBuilder<ICalcResult>()
				.Update()
				.SetValue(c => c.StatusCode, (byte)CalcResultStatusCode.Aborted)
				.SetValue(c => c.StatusName, CalcResultStatusCode.Aborted.ToString())
				.SetValue(c => c.StatusNote, reason)
				.SetValue(c => c.FinishTime, DateTimeOffset.Now)
				.Where(c => c.Id, ConditionOperator.Equal, resultId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcResultStatusCode.Processing);

			return calcDbScope.Executor.Execute(query) > 0;
		}

		private bool ExistsCheckPoint(IDataLayerScope dbScope, long resultId)
		{
			var selQuery = _calcServerDataLayer.GetBuilder<ICalcCheckPoint>()
				.From()
				.Select(c => c.Id)
				.Where(c => c.RESULT.Id, ConditionOperator.Equal, resultId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcCheckPointStatusCode.Available);

			var data = dbScope.Executor.ExecuteAndFetch(selQuery, reader =>
			{
				if (!reader.Read())
				{
					return false;
				}

				return true;
			});

			return data;
		}

		public void OnCompleted(ICalcContextHandle context)
		{
			//throw new System.NotImplementedException();
		}

		public void OnEvent(ICalcContextHandle context, CalcResultEvent @event)
		{
			try
			{
				var insQuery = _calcServerDataLayer.GetBuilder<ICalcResultEvent>()
						.Insert()
						.SetValue(c => c.RESULT.Id, context.ResultId)
						.SetValue(c => c.CreatedDate, DateTimeOffset.Now)
						.SetValue(c => c.LevelCode, (byte)@event.Level)
						.SetValue(c => c.LevelName, @event.Level.ToString())
						.SetValue(c => c.Context, @event.Context)
						.SetValue(c => c.Message, @event.Message)
					;

				using (var dbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>())
				{
					dbScope.Executor.Execute(insQuery);
				}
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.Subscribing, e, this);
			}
		}

		public void OnEvent<TData>(ICalcContextHandle context, CalcResultEvent<TData> @event)
		{
			try
			{
				var type = typeof(TData);
				var insQuery = _calcServerDataLayer.GetBuilder<ICalcResultEvent>()
						.Insert()
						.SetValue(c => c.RESULT.Id, context.ResultId)
						.SetValue(c => c.CreatedDate, DateTimeOffset.Now)
						.SetValue(c => c.LevelCode, (byte) @event.Level)
						.SetValue(c => c.LevelName, @event.Level.ToString())
						.SetValue(c => c.Context, @event.Context)
						.SetValue(c => c.Message, @event.Message)
						.SetValue(c => c.DataType, type.Name)
						.SetValueAsJson(c => c.DataJson, @event.Data)
					;

				using (var dbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>())
				{
					dbScope.Executor.Execute(insQuery);
				}
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.Subscribing, e, this);
			}
		}

		//public void OnEvent(ICalcContextHandle context, ICalcEvent @event)
		//{
		//	//throw new System.NotImplementedException();
		//}
	}
}
