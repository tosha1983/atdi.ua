using System;
using Atdi.Platform;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeepServices.GN06
{
    public class EstimationAssignmentsService : IGn06Service
    {
        private readonly IEarthGeometricService _earthGeometricService;
        public EstimationAssignmentsService(IEarthGeometricService earthGeometricService)
        {
            this._earthGeometricService = earthGeometricService;
        }


        public void EstimationAssignmentsPointsForEtalonNetwork(in EstimationAssignmentsPointsArgs  estimationAssignmentsPointsArgs, ref PointWithAzimuthResult pointWithAzimuthResult)
        {
             EstimationAssignmentsCalculation.Calc(in estimationAssignmentsPointsArgs,  ref pointWithAzimuthResult, this._earthGeometricService);
        }

        public void GetEtalonBroadcastingAssignmentFromAllotment(BroadcastingAllotment broadcastingAllotmentArgs, BroadcastingAssignment broadcastingAssignmentResult)
        {
            EtalonBroadcastingAssignmentFromAllotment.Calc(broadcastingAllotmentArgs, broadcastingAssignmentResult);
        }

        public void GetStationFromBroadcastingAssignment(BroadcastingAssignment broadcastingAssignmentArgs, ref ContextStation contextStationResult)
        {
            StationFromBroadcastingAssignment.Calc(broadcastingAssignmentArgs, ref contextStationResult);
        }

        public void GetBoundaryPointsFromAllotments(in BroadcastingAllotmentWithStep broadcastingAllotmentWithStepArgs, ref Points pointsResult)
        {
            BoundaryPointsFromAllotments.Calc(this._earthGeometricService, broadcastingAllotmentWithStepArgs, ref pointsResult);
        }

        public void Dispose()
        {

        }
    }
}


