using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IPointFieldStrengthCalcResult_PK : ICalcTask_PK
	{
	}

	[Entity]
	public interface IPointFieldStrengthCalcResult : ICalcTask, IPointFieldStrengthCalcResult_PK
	{
		float FS_dBuVm { get; set; }
		float Level_dBm { get; set; }
	}

}