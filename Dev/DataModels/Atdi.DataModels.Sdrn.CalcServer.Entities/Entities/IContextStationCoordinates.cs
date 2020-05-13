using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IContextStationCoordinates_PK
	{
		long StationId { get; set; }
	}
	[Entity]
	public interface IContextStationCoordinates : IContextStationCoordinates_PK
	{
		int AtdiX { get; set; }

		int AtdiY { get; set; }

		double EpsgX { get; set; }

		double EpsgY { get; set; }
	}


	
}
