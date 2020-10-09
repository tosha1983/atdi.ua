using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
    [Serializable]
    public struct ReceivedPowerCalcResult
    {
		//public double? FS_dBuVm;
		public double? Level_dBm;
        public double? Frequency_Mhz;
        public double? Distance_km;
        public double? AntennaHeight_m;
        //public double? AntennaPatternLoss_dB;
        //public double? diffractionLoss_dB;
    }
}
