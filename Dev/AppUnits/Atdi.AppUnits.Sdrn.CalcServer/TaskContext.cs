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
		private readonly IIterationsPool _iterationsPool;

		public TaskContext(DM.CalcTaskResultData taskResult, ITaskObserver observer, IIterationsPool iterationsPool)
		{
			_taskResult = taskResult;
			_observer = observer;
			_iterationsPool = iterationsPool;

			this.ResultId = taskResult.ResultId;
			this.TaskId = taskResult.TaskId;
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

		public void SendEvent(ICalcEvent @event)
		{
			_observer.OnEvent(this, @event);
		}

		public TResult RunIteration<TData, TResult>(TData data)
		{
			var iteration = _iterationsPool.GetIteration<TData, TResult>();
			return iteration.Run(this, data);
		}
	}
}
