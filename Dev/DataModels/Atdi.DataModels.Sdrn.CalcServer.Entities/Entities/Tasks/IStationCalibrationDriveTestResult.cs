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
        long CalibrationResultId { get; set; }
    }

	[Entity]
	public interface IStationCalibrationDriveTestResult : IStationCalibrationDriveTestResult_PK
    {
        IStationCalibrationResult CALIBRATION_RESULT { get; set; }
        string ExternalSource { get; set; }
        string ExternalCode { get; set; }
        string LicenseGsid { get; set; }
        string RealGsid { get; set; }
        string ResultDriveTestStatus { get; set; }
        int CountPointsInDriveTest { get; set; }
        float MaxPercentCorellation { get; set; }
    }

}