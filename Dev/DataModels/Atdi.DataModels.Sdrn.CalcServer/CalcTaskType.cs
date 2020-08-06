using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer
{
	public enum CalcTaskType
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
        /// Расчет согласно спецификации GN06
        /// </summary>
        Gn06CalcTask = 4,

        /// <summary>
        /// Формирование сводной таблицы (Ref Spectrum) по результатам драйв-тестов
        /// </summary>
        RefSpectrumByDriveTestsCalcTask = 5,

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
		ThirdExampleTask = 103,


    }
}
