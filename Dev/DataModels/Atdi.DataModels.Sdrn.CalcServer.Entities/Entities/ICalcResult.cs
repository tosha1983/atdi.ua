using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface ICalcResult_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface ICalcResult : ICalcResult_PK
	{
		ICalcTask TASK { get; set; }

		byte StatusCode { get; set; }

		string StatusName { get; set; }

		string StatusNote { get; set; }

		string CallerInstance { get; set; }

		Guid CallerResultId { get; set; }

		DateTimeOffset CreatedDate { get; set; }

		DateTimeOffset? StartTime { get; set; }

		DateTimeOffset? FinishTime { get; set; }

	}

	public enum CalcResultStatusCode
	{
		/// <summary>
		/// Фаза создания и подготовки окружения к запуску процесса расчета
		/// </summary>
		Created = 0,

		/// <summary>
		/// Фаза ожидания запуска процесса расчета
		/// </summary>
		Pending = 1,

		/// <summary>
		/// Фаза ожидания запуска процесса расчета
		/// </summary>
		Accepted = 2,

		/// <summary>
		/// Расчет выполняется
		/// </summary>
		Processing = 3,

		/// <summary>
		/// Расчет завершен
		/// </summary>
		Completed = 4,

		/// <summary>
		/// Расчет был отменен по внешней причине
		/// </summary>
		Canceled = 5,

		/// <summary>
		/// Расчет был прерван по внутреней причине
		/// </summary>
		Aborted = 6,

		/// <summary>
		/// Попытка запуска завершилась не удачей
		/// </summary>
		Failed = 7
	}
}
