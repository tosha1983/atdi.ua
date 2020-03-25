using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IPointFieldStrengthCalcTask_PK : ICalcTask_PK
	{
	}

	[Entity]
	public interface IPointFieldStrengthCalcTask : ICalcTask, IPointFieldStrengthCalcTask_PK
	{
		IContextStation STATION { get; set; }

		double PointLongitude_DEC { get; set; }

		double PointLatitude_DEC { get; set; }

		double PointAltitude_m { get; set; }
	}
	
}
