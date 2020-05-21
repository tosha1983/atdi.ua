﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct CalibrationDriveTestResult
    {
        public long? StationId;
        public string TableName;
        public string GSIDByICSM;
        public string GSIDByDriveTest;
        public DriveTestStatusResult ResultDriveTestStatus;
        public int CountPointsInDriveTest;
        public float MaxPercentCorellation;
    }
}