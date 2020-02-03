using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppUnits.Sdrn.CalcServer.Tasks.Example.DataModels;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Example.Tasks
{
	[TaskHandler(CalcTaskType.FirstExampleTask)]
	internal class FirstTask : ITaskHandler
	{
		private readonly IDataLayer<EntityDataOrm> _dataLayer;
		private readonly ILogger _logger;
		private readonly IQueryExecutor _queryExecutor;
		private ITaskContext _taskContext;

		/// <summary>
		/// Тут заказываем то что нам нужно для работы
		/// В примере показан минимальный набор сервисов которые нужны
		/// </summary>
		/// <param name="dataLayer"></param>
		/// <param name="logger"></param>
		public FirstTask(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
		{
			// Важно: объект обработчика создается каждый раз на факт запуска

			this._dataLayer = dataLayer;
			this._logger = logger;
			this._queryExecutor = this._dataLayer.Executor<CalcServerDataContext>();
		}

		public void Dispose()
		{
			// Тут освобождаем занятые ресурсы, если такие использовали
		}

		public void Load(ITaskContext taskContext)
		{
			// фиксируем контекст
			_taskContext = taskContext;
			// загружаем и можем кешироват паркамтеры задачи
			// важно проверить все ли готово к расчету
			// если есть проблемы, можно сделать следующее:
			//   1. Сгенерировать и отправить евент с описанием пробелым, он дойдет до каллера расчета
			//   2. бросит ексепшен с описанием проблемы (это делать обязательно в случаи если расчет в принципе не возможен)

		}

		public void Run()
		{
			try
			{
				//тут код организации процесса расчета

				var firstData = new FirstIterationData();
				var firstResult = _taskContext.RunIteration<FirstIterationData, FirstIterationResult>(firstData);
				
				// в процессе можно генерировать евенты
				_taskContext.SendEvent(new CalcEvent{ Message = "Some step", Percent = 10});
			}
			catch (Exception e)
			{
				// можно запротоклировать все что нужно
				_logger.Exception("", "", e, this);

				// НО!!! Обязательно бросаем ексепшен что расчет завершился не удачно, можно указать причину
				// этот ексепшен будет запротоклирован средой а вызывающая сторона получит евент

				// поэтому, отправлять евент не обязательно, но если нужно что то пердать  больше чемс сделает сред, не запрещенно это джлать  
				throw;
			}
		}
	}
}
