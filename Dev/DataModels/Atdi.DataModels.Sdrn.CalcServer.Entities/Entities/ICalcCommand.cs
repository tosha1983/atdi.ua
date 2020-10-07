using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface ICalcCommand_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface ICalcCommand : ICalcCommand_PK
	{

		int TypeCode { get; set; }

		string TypeName { get; set; }

		byte StatusCode { get; set; }

		string StatusName { get; set; }

		string StatusNote { get; set; }

		string CallerInstance { get; set; }

		Guid CallerCommandId { get; set; }

		DateTimeOffset CreatedDate { get; set; }

		DateTimeOffset? StartTime { get; set; }

		DateTimeOffset? FinishTime { get; set; }

		string ArgsJson { get; set; }

		string ResultJson { get; set; }
	}

	public enum CalcCommandStatusCode
	{
		/// <summary>
		/// Фаза создания и подготовки окружения к запуску комманды.
		/// Сервер игнорирует этот статус и комманда в этом статусе ему не еще интересна
		/// </summary>
		Created = 0,

		/// <summary>
		/// Фаза готовности и ожидания выполнения команды.
		/// Сервер ожидает комманды в таком статусе 
		/// </summary>
		Pending = 1,

		/// <summary>
		/// Расчет выполняется
		/// </summary>
		Executing = 2,

		/// <summary>
		/// Команда удачно завершила свое выполнение
		/// </summary>
		Completed = 3,

		/// <summary>
		/// Команда отменена по внешней причине
		/// </summary>
		Canceled = 4,

		/// <summary>
		/// Команда была прервана по внутреней причине
		/// </summary>
		Aborted = 5,

		/// <summary>
		/// Выполнение команды завершилась не удачей
		/// </summary>
		Failed = 6
	}

	public enum CalcCommandTypeCode
	{
		/// <summary>
		/// Комманда "честной" остановки выполнения запущенной ранее задачи 
		/// </summary>
		CancelCalcTask= 1,

		/// <summary>
		/// Комманда прерывания выполнения запущенной ранее задачи 
		/// </summary>
		AbortCalcTask = 2,

    }

	public class CancelCalcTaskCommand
	{
		public long ResultId;
	}
	public class AbortCalcTaskCommand
	{
		public long ResultId;
	}
}
