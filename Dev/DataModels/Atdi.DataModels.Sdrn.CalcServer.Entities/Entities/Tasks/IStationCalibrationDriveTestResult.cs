﻿using System;
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
        IStationCalibrationResult CALCRESULTS_STATION_CALIBRATION { get; set; }
        long CalibrationResultId { get; set; }
        long DriveTestId { get; set; }
        long LinkToStationMonitoringId { get; set; }
        string ExternalSource { get; set; }
        string ExternalCode { get; set; }
        string StationGcid { get; set; }
        string MeasGcid { get; set; }
        string ResultDriveTestStatus { get; set; }
        int CountPointsInDriveTest { get; set; }
        float MaxPercentCorellation { get; set; }
        float Freq_MHz { get; set; }
        string Standard { get; set; }
    }

}