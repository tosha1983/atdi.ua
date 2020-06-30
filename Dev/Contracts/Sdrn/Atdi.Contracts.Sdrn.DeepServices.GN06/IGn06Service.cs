using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;


namespace Atdi.Contracts.Sdrn.DeepServices.GN06
{
	public interface IGn06Service
	{
        void EstimationAssignmentsPointsForEtalonNetwork(in EstimationAssignmentsPointsArgs estimationAssignmentsPointsArgs, ref PointWithAzimuthResult pointWithAzimuthResult);

        void GetEtalonBroadcastingAssignmentFromAllotment(BroadcastingAllotment broadcastingAllotmentArgs, BroadcastingAssignment broadcastingAssignmentResult);

        void GetStationFromBroadcastingAssignment(BroadcastingAssignment broadcastingAssignmentArgs, ref ContextStation contextStationResult);

        void GetBoundaryPointsFromAllotments(in BroadcastingAllotmentWithStep broadcastingAllotmentWithStepArgs, ref Points pointsResult);

        void CalcBarycenterGE06(in BroadcastingCalcBarycenterGE06  broadcastingCalcBarycenterGE06, ref PointEarthGeometric coordBaryCenter);
    }
}
