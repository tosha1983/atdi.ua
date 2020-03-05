using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	
	[Entity]
	public interface IContextStationSite : IContextStation_PK
	{
		double Longitude_DEC { get; set; }

		double Latitude_DEC { get; set; }

		double Altitude_m { get; set; }
	}

}
