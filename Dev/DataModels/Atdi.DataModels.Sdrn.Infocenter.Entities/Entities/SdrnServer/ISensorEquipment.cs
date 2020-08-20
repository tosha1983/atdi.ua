using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer
{
	[EntityPrimaryKey]
	public interface ISensorEquipment_PK
	{
		long Id { get; set; }
	}

	[Entity]
	public interface ISensorEquipment : ISensorEquipment_PK
	{
		string Code { get; set; }

		string Manufacturer { get; set; }

		string Name { get; set; }

		string TechId { get; set; }

		double? LowerFreq { get; set; }

		double? UpperFreq { get; set; }

		ISensor SENSOR { get; set; }
	}
}
