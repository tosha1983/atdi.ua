﻿using System;
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


        public void EstimationAssignmentsPointsForEtalonNetwork(in BroadcastingAllotment broadcastingAllotment, in AreaPoint pointAllotment, in AreaPoint pointCalcFieldStrength, ref PointWithAzimuth[] pointResult, out int sizeResultBuffer)
        {
             EstimationAssignmentsCalculation.Calc(in broadcastingAllotment, in pointAllotment, in pointCalcFieldStrength, ref pointResult, this._earthGeometricService, out sizeResultBuffer);
        }

        public void Dispose()
        {

        }
    }
}
