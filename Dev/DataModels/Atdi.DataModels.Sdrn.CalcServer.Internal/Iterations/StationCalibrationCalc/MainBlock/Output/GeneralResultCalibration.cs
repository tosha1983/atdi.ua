using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct GeneralResultCalibration
    {
        public long IdResult;
        public DateTimeOffset TimeStart;
        public string AreaName;
        public GeneralParameters  GeneralParameters;
        public int CountStation;
        public int CountStationInContour;
        public int CountStation_CS;
        public int CountStation_NS;
        public int CountStation_IT;
        public int CountStation_NF;
        public int CountStation_UN;
        public int CountGSID;
        public int CountGSID_LS;
        public int CountGSID_IT;
    }
}