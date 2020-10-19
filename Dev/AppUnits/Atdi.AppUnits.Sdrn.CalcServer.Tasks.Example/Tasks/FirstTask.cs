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
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Example.Tasks
{
	[TaskHandler(CalcTaskType.FirstExampleTask)]
	internal class FirstTask : ITaskHandler
	{
		private readonly IDataLayer<EntityDataOrm> _dataLayer;
		private readonly IIterationsPool _iterationsPool;
		private readonly ILogger _logger;
		private readonly IQueryExecutor _queryExecutor;
		private ITaskContext _taskContext;

		/// <summary>
		/// Тут заказываем то что нам нужно для работы
		/// В примере показан минимальный набор сервисов которые нужны
		/// </summary>
		/// <param name="dataLayer"></param>
		/// <param name="iterationsPool"></param>
		/// <param name="logger"></param>
		public FirstTask(IDataLayer<EntityDataOrm> dataLayer, IIterationsPool iterationsPool, ILogger logger)
		{
			// Важно: объект обработчика создается каждый раз на факт запуска

			this._dataLayer = dataLayer;
			this._iterationsPool = iterationsPool;
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

			if (taskContext.RunMode == TaskRunningMode.Recovery)
			{
				// режим восстановления
			}

		}

		public void Run()
		{
			try
			{
				FirstIterationData firstData = null;

				// проверим режим запсука
				if (_taskContext.RunMode == TaskRunningMode.Normal)
				{
					// запсук с нул
					firstData = new FirstIterationData();
				}
				else if (_taskContext.RunMode == TaskRunningMode.Recovery)
				{
					// нужно востановиться
					var lastCheckPoint = _taskContext.GetLastCheckPoint();
					if ("main".Equals(lastCheckPoint.Name))
					{
						firstData = lastCheckPoint.RestoreData<FirstIterationData>("context_first_data");
					}
					if ("main-1".Equals(lastCheckPoint.Name))
					{
						firstData = lastCheckPoint.RestoreData<FirstIterationData>("context_second_data");
					}
					// и так далее по всем возможным точкам востановлениям
				}
				//тут код организации процесса расчета

				
				var firstResult = _iterationsPool.GetIteration<FirstIterationData, FirstIterationResult>().Run(_taskContext, firstData);

				// в процессе можно генерировать евенты

				// так сообщим клиенту как мы будет двигать прогепс
				_taskContext.SendEvent(new CalcResultEvent<ProgressOption>
				{
					Level = CalcResultEventLevel.Info,
					Context = "Итерация №123",
					Message = "Некоторое сообщение",
					Data = new ProgressOption
					{
						Bound = 100, // до 100 %
						Unit = "%"
					}
				});

				_taskContext.SendEvent(new CalcResultEvent<CurrentProgress>
				{
					Level = CalcResultEventLevel.Info,
					Context = "Итерация №123",
					Message = "Некоторое сообщение",
					Data = new CurrentProgress
					{
						State = 10 // 10%
					}
				});

				for (int i = 0; i < 1000; i++)
				{
					// пример обработки запроса на отмену
					if (_taskContext.CancellationToken.IsCancellationRequested)
					{
						// нас просят прервать работу
						// тут  возможно стоит сфомировть тчоку восстановления
						using (var checkPoint = _taskContext.CreateCheckPoint("main"))
						{
							checkPoint.SaveData("context_first_data", firstData);
							checkPoint.SaveData("context_second_data", firstData);
							checkPoint.SaveData("context_third_data", firstData);
							// фиксируем контрольную точку
							checkPoint.Commit();
							// тогда возможно реанимироать и продолжить расчет
						}

						// но можно и просто выйти - все завист от логики задачи и ее объема выполняемых работ и расчетов
						return;
						// важно - ексепшен не стоит бросать так как результат получит не свойственный ситуации статус - Failed
					}
				}
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
