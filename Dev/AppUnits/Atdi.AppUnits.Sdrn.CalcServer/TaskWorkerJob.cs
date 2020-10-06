using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;
using DM = Atdi.AppUnits.Sdrn.CalcServer.DataModel;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	class TaskWorkerContext
	{
		public long ResultId;
		public ITaskObserver TaskObserver;
		public CancellationTokenSource CancellationSource;
		public TaskRunningMode RunMode;
	}

	internal class TaskWorkerJob : IJobExecutor<TaskWorkerContext>
	{
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
		private readonly ITasksFactory _tasksFactory;
		private readonly ILogger _logger;


		public TaskWorkerJob(
			IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
			ITasksFactory tasksFactory, 
			ILogger logger)
		{
			_calcServerDataLayer = calcServerDataLayer;
			_tasksFactory = tasksFactory;
			_logger = logger;
		}

		public JobExecutionResult Execute(JobExecutionContext context, TaskWorkerContext state)
		{
			try
			{
				using (var calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>())
				{
					// читаем состоянеи результата
					var taskResultData = this.LoadTaskResultData(calcDbScope, state.ResultId);

					// валидируем состояние
					CheckTaskResultStatus(taskResultData, state.RunMode);

					// готовим обработчик
					var handler = _tasksFactory.Create(taskResultData.TaskType);

					if (state.RunMode == TaskRunningMode.Normal)
					{
						//меняем статус 
						this.ChangeTaskResultStatusToProcessing(calcDbScope, state.ResultId);
					}
					else if (state.RunMode == TaskRunningMode.Normal)
					{
						// мы уже в нужном статусе, ничего менять не нужно - пока...
						_logger.Info(Contexts.ThisComponent, Categories.TaskRunning, $"The calculation task will be recovered: Result #{state.ResultId}");
					}

					TaskContext taskContext = null;
					// загружаем задачу
					try
					{
						taskContext = new TaskContext(taskResultData, state.TaskObserver, state.RunMode, state.CancellationSource.Token, this._calcServerDataLayer);
						handler.Load(taskContext);
					}
					catch (ThreadAbortException)
					{
						Thread.ResetAbort();
						this.ChangeTaskResultStatusToAborted(calcDbScope, state.ResultId, "The calculation task was aborted");
						_logger.Warning(Contexts.ThisComponent, Categories.TaskRunning, $"The calculation task was aborted: Result #{state.ResultId}");
						return JobExecutionResult.Completed;
					}
					catch (Exception e)
					{
						this.ChangeTaskResultStatus(calcDbScope, state.ResultId, CalcResultStatusCode.Failed, e.ToString());
						throw;
					}

					// выполняем расчет
					try
					{
						System.Diagnostics.Debug.WriteLine($"EXECUTED TASK: ResultID=#{state.ResultId}");
						var timer = System.Diagnostics.Stopwatch.StartNew();
						handler.Run();
						timer.Stop();
						System.Diagnostics.Debug.WriteLine(
							$"EXECUTED TASK: {timer.Elapsed.TotalMilliseconds}ms/{timer.Elapsed.TotalSeconds}sec");
					}
					catch (ThreadAbortException)
					{
						Thread.ResetAbort();
						this.ChangeTaskResultStatusToAborted(calcDbScope, state.ResultId, "The calculation task was aborted");
						_logger.Warning( Contexts.ThisComponent, Categories.TaskRunning, $"The calculation task was aborted: Result #{state.ResultId}");
						return JobExecutionResult.Completed;
					}
					catch (Exception e)
					{
						this.ChangeTaskResultStatus(calcDbScope, state.ResultId, CalcResultStatusCode.Aborted, e.ToString());
						throw;
					}

					if (!taskContext.CancellationToken.IsCancellationRequested)
					{
						// меняем статус
						this.ChangeTaskResultStatusToCompleted(calcDbScope, state.ResultId);
					}
					else
					{
						// меняем статус н аотмену
						this.ChangeTaskResultStatusToCanceled(calcDbScope, state.ResultId, "The calculation task was canceled");
						_logger.Warning(Contexts.ThisComponent, Categories.TaskRunning, $"The calculation task was canceled: Result #{state.ResultId}");
					}
				}
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.TaskRunning, $"Something went wrong while completing a task: Result #{state.ResultId}", e);
			}

			return JobExecutionResult.Completed;
		}

		private static void CheckTaskResultStatus(DM.CalcTaskResultData taskResultData, TaskRunningMode runningMode)
		{
			if (taskResultData.TaskStatus != CalcTaskStatusCode.Available)
			{
				throw new InvalidOperationException($"Invalid the current task status '{taskResultData.ResultStatus}'. Expected is Available.");
			}
			if (runningMode == TaskRunningMode.Normal && taskResultData.ResultStatus != CalcResultStatusCode.Accepted)
			{
				throw new InvalidOperationException($"Invalid the current task result status '{taskResultData.ResultStatus}'. Expected is Accepted for normal running mode.");
			}
			if (runningMode == TaskRunningMode.Recovery && taskResultData.ResultStatus != CalcResultStatusCode.Processing)
			{
				throw new InvalidOperationException($"Invalid the current task result status '{taskResultData.ResultStatus}'. Expected is Processing for recovery running mode.");
			}
		}

		private bool ChangeTaskResultStatus(IDataLayerScope calcDbScope, long resultId, CalcResultStatusCode newStatus, string statusNote = null)
		{
			var query = _calcServerDataLayer.GetBuilder<ICalcResult>()
				.Update()
				.SetValue(c => c.StatusCode, (byte)newStatus)
				.SetValue(c => c.StatusName, newStatus.ToString())
				.SetValue(c => c.StatusNote, statusNote)
				.Where(c => c.Id, ConditionOperator.Equal, resultId);

			return calcDbScope.Executor.Execute(query) > 0;
		}

		private bool ChangeTaskResultStatus(IDataLayerScope calcDbScope, long resultId, CalcResultStatusCode newStatus, CalcResultStatusCode oldStatus, string statusNote = null)
		{
			var query = _calcServerDataLayer.GetBuilder<ICalcResult>()
				.Update()
				.SetValue(c => c.StatusCode, (byte)newStatus)
				.SetValue(c => c.StatusName, newStatus.ToString())
				.SetValue(c => c.StatusNote, statusNote)
				.Where(c => c.Id, ConditionOperator.Equal, resultId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)oldStatus);

			return calcDbScope.Executor.Execute(query) > 0;
		}

		private bool ChangeTaskResultStatusToProcessing(IDataLayerScope calcDbScope, long resultId)
		{
			var query = _calcServerDataLayer.GetBuilder<ICalcResult>()
				.Update()
				.SetValue(c => c.StatusCode, (byte)CalcResultStatusCode.Processing)
				.SetValue(c => c.StatusName, CalcResultStatusCode.Processing.ToString())
				.SetValue(c => c.StartTime, DateTimeOffset.Now)
				.SetValue(c => c.StatusNote, null)
				.Where(c => c.Id, ConditionOperator.Equal, resultId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcResultStatusCode.Accepted);

			return calcDbScope.Executor.Execute(query) > 0;
		}

		private bool ChangeTaskResultStatusToCompleted(IDataLayerScope calcDbScope, long resultId)
		{
			var query = _calcServerDataLayer.GetBuilder<ICalcResult>()
				.Update()
				.SetValue(c => c.StatusCode, (byte)CalcResultStatusCode.Completed)
				.SetValue(c => c.StatusName, CalcResultStatusCode.Completed.ToString())
				.SetValue(c => c.FinishTime, DateTimeOffset.Now)
				.SetValue(c => c.StatusNote, null)
				.Where(c => c.Id, ConditionOperator.Equal, resultId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcResultStatusCode.Processing);

			return calcDbScope.Executor.Execute(query) > 0;
		}

		private bool ChangeTaskResultStatusToCanceled(IDataLayerScope calcDbScope, long resultId, string reason)
		{
			var query = _calcServerDataLayer.GetBuilder<ICalcResult>()
				.Update()
				.SetValue(c => c.StatusCode, (byte)CalcResultStatusCode.Canceled)
				.SetValue(c => c.StatusName, CalcResultStatusCode.Canceled.ToString())
				.SetValue(c => c.StatusNote, reason)
				.SetValue(c => c.FinishTime, DateTimeOffset.Now)
				.Where(c => c.Id, ConditionOperator.Equal, resultId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcResultStatusCode.Processing);

			return calcDbScope.Executor.Execute(query) > 0;
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

		private DM.CalcTaskResultData LoadTaskResultData(IDataLayerScope calcDbScope, long resultId)
		{
			var query = _calcServerDataLayer.GetBuilder<ICalcResult>()
				.From()
				.Select(
					c => c.Id,
					c => c.TASK.Id,
					c => c.TASK.StatusCode,
					c => c.TASK.OwnerTaskId,
					c => c.TASK.OwnerInstance,
					c => c.TASK.CONTEXT.Id,
					c => c.TASK.CONTEXT.PROJECT.Id,
					c => c.TASK.TypeCode,
					c => c.StatusCode,
					c => c.CallerResultId,
					c => c.CallerInstance
				)
				.Where(c => c.Id, ConditionOperator.Equal, resultId);

			return calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				DM.CalcTaskResultData data = null;
				while (reader.Read())
				{
					data = new DM.CalcTaskResultData()
					{
						ResultId = reader.GetValue(c => c.Id),
						TaskId = reader.GetValue(c => c.TASK.Id),
						TaskStatus = (CalcTaskStatusCode)reader.GetValue(c => c.TASK.StatusCode),
						TaskOwnerTaskId = reader.GetValue(c => c.TASK.OwnerTaskId),
						TaskOwnerInstance = reader.GetValue(c => c.TASK.OwnerInstance),
						ContextId = reader.GetValue(c => c.TASK.CONTEXT.Id),
						ProjectId = reader.GetValue(c => c.TASK.CONTEXT.PROJECT.Id),
						TaskType = (CalcTaskType)reader.GetValue(c => c.TASK.TypeCode),
						ResultStatus = (CalcResultStatusCode)reader.GetValue(c => c.StatusCode),
						CallerResultId = reader.GetValue(c => c.CallerResultId),
						CallerInstance = reader.GetValue(c => c.CallerInstance)
					};

				}
				return data;
			});
		}

	}
}
