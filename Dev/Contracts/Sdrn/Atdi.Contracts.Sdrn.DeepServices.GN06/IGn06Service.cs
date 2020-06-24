using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.GN06;

namespace Atdi.Contracts.Sdrn.DeepServices.GN06
{
	public interface IGn06Service
	{
        void EstimationAssignmentsPointsForEtalonNetwork(in BroadcastingAllotment  broadcastingAllotment, in AreaPoint pointAllotment, in AreaPoint pointCalcFieldStrength, ref PointWithAzimuth[] pointResult, out int sizeResultBuffer);
    }
}
