using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{

	[Entity]
	public interface IPointFieldStrengthArgsBase
	{
		IContextStation STATION { get; set; }

		double? PointLongitude_DEC { get; set; }

		double? PointLatitude_DEC { get; set; }

		double? PointAltitude_m { get; set; }
	}
	
}
