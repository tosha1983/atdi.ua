using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer
{
	[EntityPrimaryKey]
	public interface ISensorAntenna_PK
	{
		long Id { get; set; }
	}

	[Entity]
	public interface ISensorAntenna : ISensorAntenna_PK
	{
		string Code { get; set; }

		string Manufacturer { get; set; }

		string Name { get; set; }

		string TechId { get; set; }

		string AntDir { get; set; }

		double? HbeamWidth { get; set; }

		double? VbeamWidth { get; set; }

		string Polarization { get; set; }
		
		string GainType { get; set; }

		double? GainMax { get; set; }

		double? LowerFreq { get; set; }

		double? UpperFreq { get; set; }

		double? AddLoss { get; set; }

		double? Xpd { get; set; }
		
		ISensor SENSOR { get; set; }
	}
}
