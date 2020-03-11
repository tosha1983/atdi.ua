using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		private readonly IJobBroker _jobBroker;
		private readonly ILogger _logger;

		public TaskDispatcher(
			IJobBroker jobBroker,
			ILogger logger)
		{
			_jobBroker = jobBroker;
			_logger = logger;
		}

		public void RunTask(TaskLaunchHandle launchHandle, ITaskObserver observer)
		{
			// создать запись о результатах
			// перевести ее в Pending, все задачу запустят потом
		}

		public void RunTask(long resultId, ITaskObserver observer)
		{
			try
			{
				_logger.Verbouse(Contexts.ThisComponent, Categories.MapPreparation, $"Running calc task for the result with id #{resultId}");

				var taskWorkerContext = new TaskWorkerContext
				{
					ResultId = resultId,
					TaskObserver = observer
				};

				var taskWorkerJobDef = new JobDefinition<TaskWorkerJob, TaskWorkerContext>()
				{
					Name = $"Task Worker - result Id #{resultId}",
					Recoverable = false,
					Repeatable = false
				};

				_jobBroker.Run(taskWorkerJobDef, taskWorkerContext);
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.TaskRunning, $"Something went wrong when starting the calculation for the result with Id #{resultId}", e);
			}
		}

		
		public void Dispose()
		{
			//throw new NotImplementedException();
		}

		
	}
}
