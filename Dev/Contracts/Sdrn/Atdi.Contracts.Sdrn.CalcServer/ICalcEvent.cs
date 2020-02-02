namespace Atdi.Contracts.Sdrn.CalcServer
{
	public interface ICalcEvent
	{
		/// <summary>
		/// детальнео описание состояния процесса
		/// Может быт передано в пользовательский интерфейс 
		/// </summary>
		string Message { get; }

		/// <summary>
		/// Процент выполнения
		/// </summary>
		float Percent { get; }

		/// <summary>
		/// Текущий шаг
		/// </summary>
		int CurrentStep { get; }

		/// <summary>
		/// Кол-во шагов
		/// </summary>
		int StepCount { get; }

		/// <summary>
		/// Текстовое описание текущей фазы
		/// </summary>
		string Context { get; }

		/// <summary>
		/// Код текущей фазы
		/// Идентифицирует итерацию расчета 
		/// </summary>
		long IterationCode { get; }
	}

	public sealed class CalcEvent : ICalcEvent
	{
		public string Message { get; set; }
		public float Percent { get; set; }
		public int CurrentStep { get; set; }
		public int StepCount { get; set; }
		public string Context { get; set; }
		public long IterationCode { get; set; }
	}
}