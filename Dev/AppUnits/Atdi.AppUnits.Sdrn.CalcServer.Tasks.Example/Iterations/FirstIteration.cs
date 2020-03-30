using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppUnits.Sdrn.CalcServer.Tasks.Example.DataModels;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Example.Iterations
{
	/// <summary>
	/// Пример реализации итерации
	/// Объект создается один раз, т.е. его состояние глобальное
	/// это нужно учитывать при работе с БД если будет в таком необходимость
	/// т.е. открытваь всегда новое соединение в рамках вызова.  
	/// </summary>
	class FirstIteration : IIterationHandler<FirstIterationData, FirstIterationResult>
	{
		private readonly IExampleDeepService _exampleDeepService;
		private readonly IDataLayer<EntityDataOrm> _dataLayer;
		private readonly ILogger _logger;

		/// <summary>
		/// Заказываем у контейнера нужные сервисы
		/// </summary>
		public FirstIteration(IExampleDeepService exampleDeepService, IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
		{
			_exampleDeepService = exampleDeepService;
			_dataLayer = dataLayer;
			_logger = logger;
		}

		public FirstIterationResult Run(ITaskContext taskContext, FirstIterationData data)
		{
			// вычисления, можно бросить эксепше, он будет средйо положен влог и проброшен вызывающей стороне

			// можно обращаться к БД
			using (var scope = _dataLayer.CreateScope<CalcServerDataContext>())
			{
				
			}

			// можно вызват другую итерацию
			var secondData = new SecondIterationData();
			//var secondResult = taskContext.RunIteration<SecondIterationData, SecondIterationResult>(secondData);
			
			// выполняем низковроневый расчет
			var ar1 = new float[100];
			var ar2 = new float[100];
			var res1 = _exampleDeepService.CalcSomething(ar1, ar2, 50);

			// можем писать в лог
			_logger.Info("Context", "Category", "Iteration message");

			// можем бросать эксепшен, он будет передан каллеру итерации (таску или другой итерации если оны вызвала эту)
			if (false)
			{
				throw new InvalidOperationException("Something went wrong on iteration");
			}

			// обязательный возврат результата
			return new FirstIterationResult();
		}
	}
}
