using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface IStationCalibrationDriveTestResult_PK
    {
		long Id { get; set; }
	}

	[Entity]
	public interface IStationCalibrationDriveTestResult : IStationCalibrationDriveTestResult_PK
    {
        IStationCalibrationResult CALIBRATION_RESULT { get; set; }
        long? StationId { get; set; }
        string TableName { get; set; }
        string GSIDByICSM { get; set; }
        string GSIDByDriveTest { get; set; }
        string ResultDriveTestStatus { get; set; }
        int CountPointsInDriveTest { get; set; }
        float MaxPercentCorellation { get; set; }
    }

}