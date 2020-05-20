using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface ICalibrationStationResult_PK
    {
		long ResultId { get; set; }
	}

	[Entity]
	public interface ICalibrationStationResult : ICalibrationStationResult_PK
    {
        ICalibrationResult CALIBRATION_RESULT { get; set; }
        string TableName { get; set; }
        long? IdStation { get; set; }
        string GSIDByICSM { get; set; }
        string GSIDByMeasurement { get; set; }
        string ResultStationStatus { get; set; }
        double MaxCorellation { get; set; }
    }

}