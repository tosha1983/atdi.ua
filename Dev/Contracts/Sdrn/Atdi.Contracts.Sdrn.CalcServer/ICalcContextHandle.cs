using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer;

namespace Atdi.Contracts.Sdrn.CalcServer
{
	public enum CalculationStatus
	{
		/// <summary>
		/// Фаза создания записи результатов расчета
		/// Расчет небыл еще запущен
		/// </summary>
		Created = 0,
		
		/// <summary>
		/// Расчет выполняется
		/// </summary>
		Processing = 1,

		/// <summary>
		/// Расчет завершен
		/// </summary>
		Completed = 2,

		/// <summary>
		/// Расчет был отменен по внешней причине
		/// </summary>
		Canceled = 3,

		/// <summary>
		/// Расчет был прерван по внутреней причине
		/// </summary>
		Aborted = 4,

		/// <summary>
		/// Попытка запуска завершилась не удачей
		/// </summary>
		Failed = 5
	}

	/// <summary>
	/// Описатель контекста расчета
	/// </summary>
	public interface ICalcContextHandle
	{
		long ResultId { get; }

		TaskLaunchHandle LaunchHandle { get; }

		CalculationStatus Status { get; }
	} 
}
