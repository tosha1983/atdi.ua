using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface ICalibrationStationParamByICSMResult_PK
    {
		long ParamId { get; set; }
	}

	[Entity]
	public interface ICalibrationStationNewParamResult : ICalibrationStationParamBase, ICalibrationStationParamByICSMResult_PK
    {

    }

}