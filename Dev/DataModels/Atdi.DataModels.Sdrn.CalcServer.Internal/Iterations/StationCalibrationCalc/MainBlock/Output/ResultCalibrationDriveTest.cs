using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct ResultCalibrationDriveTest
    {
        public string TableName;
        public long IdStation;
        public string GSIDByICSM;
        public string GSIDByDriveTest;
        public string ResultDriveTestStatus;
        public double CountPointsInDriveTest;
        public double MaxPercentCorellation;
        public ParametersStation ParametersStationbyICSM;
        public ParametersStation ParametersStationbyMeasurement;
        public DriveTestsResult DriveTestsResult;
        public int CountStation_LS;
        public int CountStation_IT;
        public int CountStation_UN;
    }
}