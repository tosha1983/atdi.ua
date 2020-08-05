using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer
{
	[EntityPrimaryKey]
	public interface ISensor_PK
	{
		long Id { get; set; }
	}

	[Entity]
	public interface ISensor : ISensor_PK
	{
		long? SensorIdentifierId { get; set; }

		string Status { get; set; }

		string Name { get; set; }

		DateTime? BiuseDate { get; set; }

		DateTime? EouseDate { get; set; }

		double? Azimuth { get; set; }

		double? Elevation { get; set; }

		double? Agl { get; set; }

		double? RxLoss { get; set; }

		string TechId { get; set; }
	}
}
