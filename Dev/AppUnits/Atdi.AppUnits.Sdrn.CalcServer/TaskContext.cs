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
using DM = Atdi.AppUnits.Sdrn.CalcServer.DataModel;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	class TaskContext : ITaskContext, ICalcContextHandle
	{
		private readonly DM.CalcTaskResultData _taskResult;
		private readonly ITaskObserver _observer;
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;

		public TaskContext(DM.CalcTaskResultData taskResult, ITaskObserver observer, TaskRunningMode runMode, CancellationToken cancellationToken, IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer)
		{
			_taskResult = taskResult;
			_observer = observer;
			_calcServerDataLayer = calcServerDataLayer;

			this.ResultId = taskResult.ResultId;
			this.TaskId = taskResult.TaskId;
			this.ProjectId = taskResult.ProjectId;
			this.ClientContextId = taskResult.ContextId;
			this.RunMode = runMode;
			this.CancellationToken = cancellationToken;

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

		public TaskRunningMode RunMode { get; }

		public CancellationToken CancellationToken { get; }

		public ICheckPoint CreateCheckPoint(string name)
		{
			var calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>();
			var insQuery = _calcServerDataLayer.GetBuilder<ICalcCheckPoint>()
				.Insert()
				.SetValue(c => c.Name, name)
				.SetValue(c => c.StatusCode, (byte)CalcCheckPointStatusCode.Created)
				.SetValue(c => c.StatusName, CalcCheckPointStatusCode.Created.ToString())
				.SetValue(c => c.RESULT.Id, this.ResultId)
				.SetValue(c => c.CreatedDate, DateTimeOffset.Now);

			var pk = calcDbScope.Executor.Execute<ICalcCheckPoint_PK>(insQuery);

			var result = new CheckPoint(pk.Id, name, false, _calcServerDataLayer, calcDbScope);
			return result;
		}

		public ICheckPoint GetLastCheckPoint()
		{
			var calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>();
			var selQuery = _calcServerDataLayer.GetBuilder<ICalcCheckPoint>()
				.From()
				.Select(c => c.Id)
				.Select(c => c.Name)
				.Where(c => c.RESULT.Id, ConditionOperator.Equal, this.ResultId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte) CalcCheckPointStatusCode.Available)
				.OrderByDesc(c => c.Id);

			var checkPoint = calcDbScope.Executor.ExecuteAndFetch(selQuery, reader =>
			{
				if (reader.Read())
				{
					var result = new CheckPoint(
						reader.GetValue(c=>c.Id), 
						reader.GetValue(c => c.Name), 
						true, _calcServerDataLayer, calcDbScope);

					return result;
				}

				return null;
			});

			return checkPoint;
		}

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
