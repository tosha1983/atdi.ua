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
}
