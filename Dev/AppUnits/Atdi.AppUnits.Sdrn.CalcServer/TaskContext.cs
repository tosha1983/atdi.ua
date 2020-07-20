using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer;
using DM = Atdi.AppUnits.Sdrn.CalcServer.DataModel;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	class TaskContext : ITaskContext, ICalcContextHandle
	{
		private readonly DM.CalcTaskResultData _taskResult;
		private readonly ITaskObserver _observer;

		public TaskContext(DM.CalcTaskResultData taskResult, ITaskObserver observer)
		{
			_taskResult = taskResult;
			_observer = observer;

			this.ResultId = taskResult.ResultId;
			this.TaskId = taskResult.TaskId;
			this.ProjectId = taskResult.ProjectId;
			this.ClientContextId = taskResult.ContextId;

			this.LaunchHandle = new TaskLaunchHandle
			{
				TaskId = taskResult.TaskId,
				CallerInstance = taskResult.CallerInstance,
				ResultCode = taskResult.CallerResultId
			};
			this.Status = CalculationStatus.Created;
		}

		public long ResultId { get;}

		public TaskLaunchHandle LaunchHandle { get; }

		public CalculationStatus Status { get; set; }

		public long TaskId { get; }

		public long ProjectId { get; }

		public long ClientContextId { get; }

		public void SendEvent(CalcResultEvent @event)
		{
			_observer.OnEvent(this, @event);
		}

		public void SendEvent<TData>(CalcResultEvent<TData> @event)
		{
			_observer.OnEvent(this, @event);
		}

		//public TResult RunIteration<TData, TResult>(TData data)
		//{
		//	var iteration = _iterationsPool.GetIteration<TData, TResult>();
		//	return iteration.Run(this, data);
		//}

		//public IIterationHandler<TData, TResult> GetIteration<TData, TResult>()
		//{
		//	return _iterationsPool.GetIteration<TData, TResult>();
		//}
	}
}
