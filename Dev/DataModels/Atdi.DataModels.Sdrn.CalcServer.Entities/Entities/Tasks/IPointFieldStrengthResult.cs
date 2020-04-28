using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface IPointFieldStrengthResult_PK
	{
		long ResultId { get; set; }
	}

	[Entity]
	public interface IPointFieldStrengthResult : IPointFieldStrengthResult_PK
	{
		ICalcResult RESULT { get; set; }

		float? FS_dBuVm { get; set; }

		float? Level_dBm { get; set; }
	}

}