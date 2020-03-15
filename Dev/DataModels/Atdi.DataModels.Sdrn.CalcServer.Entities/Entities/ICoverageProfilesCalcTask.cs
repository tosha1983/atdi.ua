using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface ICoverageProfilesCalcTask_PK : ICalcTask_PK
	{
		//long Id { get; set; }
	}

	[Entity]
	public interface ICoverageProfilesCalcTask : ICalcTask, ICoverageProfilesCalcTask_PK
	{
		byte ModeCode { get; set; }

		string ModeName { get; set; }

		int[] PointsX { get; set; }

		int[] PointsY { get; set; }

		string ResultPath { get; set; }
	}

	public enum CoverageProfilesCalcModeCode
	{
		
		/// <summary>
		/// Режим построения профелей последовательно, 0 с 1 , 2 с 3 и т.д. 
		/// </summary>
		InPairs = 0,

		/// <summary>
		/// Режим построения профелей с первой точки до всех остальных
		/// </summary>
		FirstWithAll = 1,

		/// <summary>
		/// Режим построения профелей всех точек со всеми
		/// </summary>
		AllWithAll = 2
	}
}
