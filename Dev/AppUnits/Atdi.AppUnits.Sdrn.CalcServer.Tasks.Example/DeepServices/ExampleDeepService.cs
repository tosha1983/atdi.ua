using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.Platform;
using Atdi.Platform.Data;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Example.DeepServices
{
	/// <summary>
	/// Реализация низкоуровнего сервиса
	/// Запрещено работат с БД, с инфратсруктуруой и прочими фишками, кроме кеширования, пулов объектов и логирования.
	/// Только математика, расчеты,
	///
	/// объект - сингл тон, реализация методов должна быть потокобезапасной
	/// не использовать состоянеи класса для хранения промежуточных результатов
	/// </summary>
	internal sealed class ExampleDeepService : IExampleDeepService
	{
		private readonly ILogger _logger;
		private readonly IObjectPoolSite _objectPoolSite;
		private readonly IStatistics _statistics;

		/// <summary>
		/// Можно заказть
		/// - другой низкоуровневый сервис
		/// - сервис пулов,
		/// - сервис статистики,
		/// - сервис пртоклирования - в основном для отладки
		/// </summary>
		public ExampleDeepService(IObjectPoolSite objectPoolSite, IStatistics statistics, ILogger logger)
		{
			_logger = logger;
			_objectPoolSite = objectPoolSite;
			_statistics = statistics;
		}

		/// <summary>
		/// Поткобезопасная реализация низкоуровневого расчета
		/// </summary>
		/// <param name="ar1"></param>
		/// <param name="ar2"></param>
		/// <param name="len"></param>
		/// <returns></returns>
		public float[] CalcSomething(float[] ar1, float[] ar2, int len)
		{
			var res = new float[len];
			for (int i = 0; i < len; i++)
			{
				res[i] = ar1[i] + ar2[i];
			}

			return res;
		}

		public double[] CalcSomething(double[] ar1, double[] ar2, int len)
		{
			var res = new double[len];
			for (int i = 0; i < len; i++)
			{
				res[i] = ar1[i] - ar2[i];
			}

			return res;
		}

		public void Dispose()
		{
			// тут освобождаем ресурсы
		}
	}
}
