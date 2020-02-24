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
