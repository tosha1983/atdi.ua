using System;
using Atdi.Platform;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeepServices.GN06
{
    public class EstimationAssignmentsService : IGn06Service
    {

        public EstimationAssignmentsService()
        {

        }


        public void EstimationAssignmentsPointsForEtalonNetwork(in BroadcastingAllotment broadcastingAllotment, in AreaPoint pointAllotment, in AreaPoint pointCalcFieldStrength, ref PointWithAzimuth[] pointResult, out int sizeResultBuffer)
        {
             EstimationAssignmentsCalculation.Calc(in broadcastingAllotment, in pointAllotment, in pointCalcFieldStrength, ref pointResult, out sizeResultBuffer);
        }

        public void Dispose()
        {

        }
    }
}
