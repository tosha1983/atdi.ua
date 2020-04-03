using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IContextStationPattern_PK
	{
		long StationId { get; set; }

		string AntennaPlane { get; set; }

		string WavePlane { get; set; }
	}
	[Entity]
	public interface IContextStationPattern : IContextStationPattern_PK
	{
		float[] Loss_dB { get; set; }

		double[] Angle_deg { get; set; }
	}


	
}
