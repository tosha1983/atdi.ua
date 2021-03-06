﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult
{
    public class StationCalibrationDriveTestsModel
    {
        public long Id { get; set; }
        public long ResultId { get; set; }
        public long LinkToStationMonitoringId { get; set; }
        public long DriveTestId { get; set; }
        public string ExternalSource { get; set; }
        public string ExternalCode { get; set; }
        public string StationGcid { get; set; }
        public string MeasGcid { get; set; }
        public string ResultDriveTestStatus { get; set; }
        public int CountPointsInDriveTest { get; set; }
        public float MaxPercentCorellation { get; set; }
        public float Freq_MHz { get; set; }
        public string Standard { get; set; }
    }
}
