using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct CalcPoint
    {
        public double X;
        public double Y;
        public int Count;
        public double FSMeas;
        public double FSCalc;
        public double DiffractionLoss_dB;
        public double Dist_km;
    }
}
