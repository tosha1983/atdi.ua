using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer
{
	[EntityPrimaryKey]
	public interface ISensorAntennaPattern_PK
	{
		long Id { get; set; }
	}

	[Entity]
	public interface ISensorAntennaPattern : ISensorAntennaPattern_PK
	{
		double? Freq { get; set; }

		double? Gain { get; set; }

		string DiagA { get; set; }

		string DiagH { get; set; }

		string DiagV { get; set; }

		ISensorAntenna SENSOR_ANTENNA { get; set; }
	}
}
