using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer
{
	[EntityPrimaryKey]
	public interface ISensorEquipmentSensitivity_PK
	{
		long Id { get; set; }
	}

	[Entity]
	public interface ISensorEquipmentSensitivity : ISensorEquipmentSensitivity_PK
	{
		double? Freq { get; set; }

		double? Ktbf { get; set; }

		double? Noisef { get; set; }

		double? FreqStability { get; set; }

		double? AddLoss { get; set; }

		ISensorEquipment SENSOR_EQUIP { get; set; }
	}
}
