using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppUnits.Sdrn.CalcServer.Tasks.Example.DataModels;
using Atdi.Contracts.Sdrn.CalcServer;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Example.Iterations
{
	/// <summary>
	/// Пример реализации итерации
	/// Объект создается один раз, т.е. его состояние глобальное
	/// это нужно учитывать при работе с БД если будет в таком необходимость
	/// т.е. открытваь всегда новое соединение в рамках вызова.  
	/// </summary>
	class SecondIteration : IIterationHandler<SecondIterationData, SecondIterationResult>
	{
		/// <summary>
		/// Заказываем у контейнера нужные сервисы
		/// </summary>
		public SecondIteration()
		{

		}

		public SecondIterationResult Run(ITaskContext taskContext, SecondIterationData data)
		{
			// вычисления, можно бросить эксепше, он будет средйо положен влог и проброшен вызывающей стороне

			// возврат результата
			return new SecondIterationResult();
		}
	}
}
