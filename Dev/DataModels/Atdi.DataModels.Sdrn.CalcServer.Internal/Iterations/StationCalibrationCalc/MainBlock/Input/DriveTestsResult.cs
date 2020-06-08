using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public class DriveTestsResult
    {
        public string GSID;

        public double Freq_MHz;

        public string Standard;

        public string RealStandard;

        public PointFS[] Points;

        public int CountPoints;

        public string NameGroupGlobalSID;

        public long Num;

        public long DriveTestId;

        public long LinkToStationMonitoringId;
    }
}
