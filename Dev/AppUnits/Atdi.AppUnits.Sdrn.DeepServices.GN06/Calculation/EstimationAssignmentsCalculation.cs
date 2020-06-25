using System;
using Atdi.Platform;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeepServices.GN06
{
    public static class EstimationAssignmentsCalculation
    {
        public static void Calc(in BroadcastingAllotment broadcastingAllotment, in AreaPoint pointAllotment, in AreaPoint pointCalcFieldStrength, ref PointWithAzimuth[] pointResult, out int sizeResultBuffer)
        {
            sizeResultBuffer = 0;
            if (broadcastingAllotment.EmissionCharacteristics.RefNetwork == RefNetworkType.RN1)
            {
                EstimationAssignmentsCalculationRN1.Calc(in broadcastingAllotment, in pointAllotment, in pointCalcFieldStrength, ref pointResult, out sizeResultBuffer);
            }
            else if (broadcastingAllotment.EmissionCharacteristics.RefNetwork == RefNetworkType.RN2)
            {
                EstimationAssignmentsCalculationRN2.Calc(in broadcastingAllotment, in pointAllotment, in pointCalcFieldStrength, ref pointResult, out sizeResultBuffer);
            }
            else if (broadcastingAllotment.EmissionCharacteristics.RefNetwork == RefNetworkType.RN3)
            {
                EstimationAssignmentsCalculationRN3.Calc(in broadcastingAllotment, in pointAllotment, in pointCalcFieldStrength, ref pointResult, out sizeResultBuffer);
            }
            else if (broadcastingAllotment.EmissionCharacteristics.RefNetwork == RefNetworkType.RN4)
            {
                EstimationAssignmentsCalculationRN4.Calc(in broadcastingAllotment, in pointAllotment, in pointCalcFieldStrength, ref pointResult, out sizeResultBuffer);
            }
        }
    }
}
