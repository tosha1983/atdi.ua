using System;
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
using DM = Atdi.AppUnits.Sdrn.CalcServer.DataModel;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	internal sealed class TaskDispatcher : ITaskDispatcher, IDisposable
	{
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
		private readonly ILogger _logger;

		public TaskDispatcher(
			IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
			ILogger logger)
		{
			_calcServerDataLayer = calcServerDataLayer;
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
				_logger.Verbouse(Contexts.ThisComponent, Categories.MapPreparation, $"Runnig calc task for the result with id #{resultId}");

				using (var calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>())
				{
					// читаем состоянеи результата
					var taskResultData = this.LoadTaskResultData(calcDbScope, resultId);

					// валидируем состояние
					CheckTaskResultStatus(taskResultData);

					//меняем статус 
					this.ChangeTaskResultStatus(calcDbScope, resultId, CalcResultStatusCode.Processing, CalcResultStatusCode.Pending);
					try
					{
						// посик обработчика
						// формирование контекста расчета
						// запуск расчета в отдельном потоке...
					}
					catch (Exception e)
					{
						// меняем состояние на Failed
						this.ChangeTaskResultStatus(calcDbScope, resultId, CalcResultStatusCode.Failed, e.ToString());

						throw;
					}
				}
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.TaskRunning, $"Something went wrong when starting the calculation for the result with Id #{resultId}", e);
			}
			//throw new NotImplementedException();
		}

		private static void CheckTaskResultStatus(DM.CalcTaskResultData taskResultData)
		{
			if (taskResultData.TaskStatus != CalcTaskStatusCode.Available)
			{
				throw new InvalidOperationException($"Invalid the current task status '{taskResultData.ResultStatus}'. Expected is Available.");
			}
			if (taskResultData.ResultStatus != CalcResultStatusCode.Pending)
			{
				throw new InvalidOperationException($"Invalid the current task result status '{taskResultData.ResultStatus}'. Expected is Pending.");
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

		public void Dispose()
		{
			//throw new NotImplementedException();
		}

		private DM.CalcTaskResultData LoadTaskResultData(IDataLayerScope calcDbScope, long resultId)
		{
			return new DM.CalcTaskResultData();
		}
	}
}
