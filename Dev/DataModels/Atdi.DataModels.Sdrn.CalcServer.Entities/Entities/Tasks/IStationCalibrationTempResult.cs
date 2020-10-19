using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface IStationCalibrationTempResult_PK
    {
        long Id { get; set; }
	}

	[Entity]
	public interface IStationCalibrationTempResult : IStationCalibrationTempResult_PK
    {
		ICalcResult RESULT { get; set; }
        byte[] Content { get; set; }
    }

}