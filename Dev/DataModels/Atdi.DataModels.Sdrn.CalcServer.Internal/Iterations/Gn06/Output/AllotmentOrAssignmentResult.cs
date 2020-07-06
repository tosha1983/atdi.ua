using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
    public class AllotmentOrAssignmentResult
    {

        public string Adm;
        public string TypeTable;
        public string Name;
        public double? Freq_MHz;
        public double? Longitude_DEC;
        public double? Latitude_DEC;
        public int? MaxEffHeight_m;
        public string Polar;
        public float? ErpH_dbW;
        public float? ErpV_dbW;
        public string AntennaDirectional;
        public string AdmRefId;
    }
}
