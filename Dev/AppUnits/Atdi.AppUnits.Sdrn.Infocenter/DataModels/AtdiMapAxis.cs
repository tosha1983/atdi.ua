using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Infocenter.DataModels
{
	internal struct AtdiMapAxis
	{
		/// <summary>
		/// Кол-во шагов
		/// </summary>
		public int Number;

		/// <summary>
		/// Размер шага в единицах карты
		/// </summary>
		public int Step;

		/// <summary>
		/// Длина оси в единицах карты
		/// </summary>
		public int Size => Step * Number;

		public override string ToString()
		{
			return $"Num = '{Number}', Step = '{Step}', Size = '{Size}'";
		}
	}
}
