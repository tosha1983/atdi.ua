using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface IStationCalibrationParametersArgs_PK
    {
		long TaskId { get; set; }
	}

	[Entity]
	public interface IStationCalibrationParametersArgs : IStationCalibrationParametersArgsBase, IStationCalibrationParametersArgs_PK
    {
        ICalcTask TASK { get; set; }
	}
	
}
