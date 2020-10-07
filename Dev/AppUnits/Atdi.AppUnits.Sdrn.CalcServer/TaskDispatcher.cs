using System;
using System.Collections.Concurrent;
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
	internal sealed class TaskDispatcher : ITaskDispatcher, IDisposable
	{
		private class TaskExecutingDescriptor
		{
			public TaskWorkerContext Context;
			public IJobToken JobToken;
		}

		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
		private readonly IJobBroker _jobBroker;
		private readonly ILogger _logger;
		private readonly Dictionary<long, TaskExecutingDescriptor> _descriptors;

		public TaskDispatcher(
			IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
			IJobBroker jobBroker,
			ILogger logger)
		{
			_calcServerDataLayer = calcServerDataLayer;
			_jobBroker = jobBroker;
			_logger = logger;
			_descriptors = new Dictionary<long, TaskExecutingDescriptor>();
		}

		public void RunTask(TaskLaunchHandle launchHandle, ITaskObserver observer)
		{
			// создать запись о результатах
			// перевести ее в Pending, все задачу запустят потом
		}

		private void RunTask(long resultId, ITaskObserver observer, TaskRunningMode runMode)
		{
			try
			{
				_logger.Verbouse(Contexts.ThisComponent, Categories.MapPreparation, $"Running calc task for the result with id #{resultId}");

				using (var calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>())
				{
					calcDbScope.BeginTran();
					try
					{
						if (runMode == TaskRunningMode.Normal)
						{
							if (!this.TryAcceptTaskResult(calcDbScope, resultId))
							{
								throw new InvalidOperationException("Failed to accept the task result record for processing. Invalid identifier value or record status.");
							}
						}
						

						var taskWorkerContext = new TaskWorkerContext
						{
							ResultId = resultId,
							TaskObserver = observer,
							CancellationSource = new CancellationTokenSource(),
							RunMode = runMode
						};

						var taskWorkerJobDef = new JobDefinition<TaskWorkerJob, TaskWorkerContext>()
						{
							Name = $"Task Worker - result Id #{resultId}",
							Recoverable = false,
							Repeatable = false
						};

						var jobToken = _jobBroker.Run(taskWorkerJobDef, taskWorkerContext);
						_descriptors.Add(resultId, new TaskExecutingDescriptor
						{
							Context =  taskWorkerContext,
							JobToken = jobToken
						});

						calcDbScope.Commit();
					}
					catch (Exception)
					{
						calcDbScope.Rollback();
						throw;
					}
					
				}
				
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.TaskRunning, $"Something went wrong when starting the calculation for the result with Id #{resultId}", e);
			}
		}

		private bool TryAcceptTaskResult(IDataLayerScope calcDbScope, long resultId)
		{
			var query = _calcServerDataLayer.GetBuilder<ICalcResult>()
				.Update()
				.SetValue(c => c.StatusCode, (byte)CalcResultStatusCode.Accepted)
				.SetValue(c => c.StatusName, "Accepted")
				.SetValue(c => c.StatusNote, "The calc result record was accepted")
				.Where(c => c.Id, ConditionOperator.Equal, resultId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcResultStatusCode.Pending);

			return calcDbScope.Executor.Execute(query) > 0;
		}

		private bool CheckTaskInProcess(IDataLayerScope calcDbScope, long resultId)
		{
			var query = _calcServerDataLayer.GetBuilder<ICalcResult>()
				.From()
				.Select(c => c.Id)
				.Where(c => c.Id, ConditionOperator.Equal, resultId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcResultStatusCode.Processing);

			return calcDbScope.Executor.Execute(query) > 0;
		}

		public void Dispose()
		{
			//throw new NotImplementedException();
		}

		public void StopTask(long resultId)
		{
			using (var calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>())
			{
				if (!CheckTaskInProcess(calcDbScope, resultId))
				{
					return;
				}

				if (_descriptors.TryGetValue(resultId, out var descriptor))
				{
					descriptor.Context.CancellationSource.Cancel();
				}
			}
				
		}

		public void AbortTask(long resultId)
		{
			using (var calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>())
			{
				if (!CheckTaskInProcess(calcDbScope, resultId))
				{
					return;
				}
				if (_descriptors.TryGetValue(resultId, out var descriptor))
				{
					descriptor.JobToken.Abort();
				}
			}
		}

		public void ResumeTask(long resultId, ITaskObserver observer)
		{
			this.RunTask(resultId, observer, TaskRunningMode.Recovery);
		}

		public void RunTask(long resultId, ITaskObserver observer)
		{
			this.RunTask(resultId, observer, TaskRunningMode.Normal);
		}
	}
}
