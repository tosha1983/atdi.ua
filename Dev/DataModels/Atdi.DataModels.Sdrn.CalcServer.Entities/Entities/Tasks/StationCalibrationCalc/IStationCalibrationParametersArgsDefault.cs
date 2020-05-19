using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface IStationCalibrationParametersArgsDefault_PK
    {
		long TaskId { get; set; }
	}

	[Entity]
	public interface IStationCalibrationParametersArgsDefault : IStationCalibrationParametersArgsBase, IStationCalibrationParametersArgsDefault_PK
    {
        IContextPlannedCalcTask TASK { get; set; }
    }
	
}
