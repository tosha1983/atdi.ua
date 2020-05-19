using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct GeneralParameters
    {
        public float СorrelationThresholdHard;
        public float СorrelationThresholdWeak;
        public float TrustOldResults;
        public bool UseMeasurementSameGSID;
        public int DistanceAroundContour_km;
        public int MinNumberPointForCorrelation;
    }
}
