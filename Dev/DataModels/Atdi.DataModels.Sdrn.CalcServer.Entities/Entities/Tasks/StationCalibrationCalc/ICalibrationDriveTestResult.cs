using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface ICalibrationDriveTestResult_PK
    {
		long ResultId { get; set; }
	}

	[Entity]
	public interface ICalibrationDriveTestResult : ICalibrationDriveTestResult_PK
    {
        ICalibrationResult CALIBRATION_RESULT { get; set; }
        long? IdStation { get; set; }
        string TableName { get; set; }
        string GSIDByICSM { get; set; }
        string GSIDByDriveTest { get; set; }
        string ResultDriveTestStatus { get; set; }
        int CountPointsInDriveTest { get; set; }
        double MaxPercentCorellation { get; set; }
    }

}