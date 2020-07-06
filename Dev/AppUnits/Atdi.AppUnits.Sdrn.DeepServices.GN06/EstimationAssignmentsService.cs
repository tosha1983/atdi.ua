using System;
using Atdi.Platform;
using Atdi.Contracts.Sdrn.DeepServices.IDWM;
using Atdi.DataModels.Sdrn.DeepServices.IDWM;
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
        private readonly IIdwmService  _idwmService;
        public EstimationAssignmentsService(IEarthGeometricService earthGeometricService, IIdwmService idwmService)
        {
            this._earthGeometricService = earthGeometricService;
            this._idwmService = idwmService;
        }


        public void EstimationAssignmentsPointsForEtalonNetwork(in EstimationAssignmentsPointsArgs  estimationAssignmentsPointsArgs, ref PointsWithAzimuthResult pointWithAzimuthResult)
        {
             EstimationAssignmentsCalculation.Calc(in estimationAssignmentsPointsArgs,  ref pointWithAzimuthResult, this._earthGeometricService);
        }

        /// <summary>
        /// Преобразование алотмента в асаймент
        /// </summary>
        /// <param name="broadcastingAllotmentArgs">Входной элотмент</param>
        /// <param name="broadcastingAssignmentResult">Выходной асаймент</param>
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

        public void CalcBarycenterGE06(in BroadcastingCalcBarycenterGE06 broadcastingCalcBarycenterGE06, ref PointEarthGeometric coordBaryCenter)
        {
            BarycenterGE06.Calc(this._earthGeometricService, this._idwmService, broadcastingCalcBarycenterGE06, ref coordBaryCenter);
        }

        public void Dispose()
        {

        }
    }
}


