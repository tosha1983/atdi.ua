using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface ICalcTask_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface ICalcTask : ICalcTask_PK
	{
		IClientContext CONTEXT { get; set; }

		int TypeCode { get; set; }

		string TypeName { get; set; }

		byte StatusCode { get; set; }

		string StatusName { get; set; }

		string StatusNote { get; set; }

		string OwnerInstance { get; set; }

		Guid OwnerTaskId { get; set; }

		DateTimeOffset CreatedDate { get; set; }

		string MapName { get; set; }
	}

	public enum CalcTaskStatusCode
	{
		/// <summary>
		/// Задача создана но не доступна для использования
		/// В этой фазе обычно формируют остальные элементы структуры задачи 
		/// </summary>
		Created = 0,
		/// <summary>
		/// Задача изменяется
		/// </summary>
		Modifying = 1,
		/// <summary>
		/// Задача полностью сформирована, парамтеры определены и ее можно использовать для иницирования рассчетов
		/// </summary>
		Available = 2,
		/// <summary>
		/// Задача временно заблокирована для иницирования рассчетов
		/// </summary>
		Locked = 3,
		/// <summary>
		/// Задача неактуальная
		/// </summary>
		Archived = 4
	}

	public enum CalcTaskTypeCode
	{
		/// <summary>
		/// Расчет профилей покрытия относительно одной произволной точки 
		/// </summary>
		CoverageProfilesCalc = 1,

		/// <summary>
		/// Расчет профилей покрытия относительно одной произволной точки 
		/// </summary>
		PointFieldStrengthCalc = 2,

        /// <summary>
        /// Определение параметров станций по результатам измерений мобильной компоненты
        /// </summary>
        StationCalibrationCalcTask = 3,


        /// <summary>
        /// Первая тестовая расчетная задача
        /// </summary>
        FirstExampleTask = 101,

		/// <summary>
		/// Вторая тестовая расчетная задача
		/// </summary>
		SecondExampleTask = 102,

		/// <summary>
		/// Треться тестовая расчетная задачи
		/// </summary>
		ThirdExampleTask = 103
        

    }
}
