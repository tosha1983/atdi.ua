using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
    [Serializable]
    public class CalibrationResult
    {
        public long IdResult;
        public string Standard;
        public DateTimeOffset TimeStart;
        public string AreaName;
        public GeneralParameters GeneralParameters;
        public int NumberStation;
        public int NumberStationInContour;
        public int CountStation_CS;
        public int CountStation_NS;
        public int CountStation_IT;
        public int CountStation_NF;
        public int CountStation_UN;
        public int CountMeasGSID;
        public int CountMeasGSID_LS;
        public int CountMeasGSID_IT;
        public CalibrationStationResult[] ResultCalibrationStation;
        public CalibrationDriveTestResult[] ResultCalibrationDriveTest;
    }
}