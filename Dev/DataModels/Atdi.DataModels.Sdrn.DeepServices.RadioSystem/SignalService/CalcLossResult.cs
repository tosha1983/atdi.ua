using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService
{
    [Serializable]
    public struct CalcLossResult
	{
		public double LossD_dB;
        public double DiffractionLoss_dB;
        public double TiltaD_Deg;
		public double TiltbD_Deg;
        public double LossA_dB;
        public double TiltaA_Deg;
        public double TiltbA_Deg;

    }
}
