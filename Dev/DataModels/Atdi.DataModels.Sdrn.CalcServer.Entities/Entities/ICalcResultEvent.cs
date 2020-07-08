using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface ICalcResultEvent_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface ICalcResultEvent : ICalcResultEvent_PK
	{
		ICalcResult RESULT { get; set; }

		DateTimeOffset CreatedDate { get; set; }

		byte LevelCode { get; set; }

		string LevelName { get; set; }

		string Context { get; set; }

		string Message { get; set; }

		string DataType { get; set; }
		
		string DataJson { get; set; }
	}

	public enum CalcResultEventLevelCode
	{
		/// <summary>
		/// Собыятие несущее полезную информацию
		/// </summary>
		Info = 0,

		/// <summary>
		/// Событие несущее информацию о предупреждении
		/// </summary>
		Warning = 1,

		/// <summary>
		/// Ошибка
		/// </summary>
		Error = 2
	}

	public class CurrentProgress
	{
		/// <summary>
		/// Состояние прогресса
		/// </summary>
		public float State;
	}

	public class ProgressOption
	{
		/// <summary>
		/// Граничное значение до которого будет идти прогресс
		/// </summary>
		public float Bound;

		/// <summary>
		/// Единица измерения значений прогресса
		/// </summary>
		public string Unit;
	}
}
