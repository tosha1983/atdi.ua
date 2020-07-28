using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct FieldStrengthCalcResult
	{
		public double? FS_dBuVm;
		public double? Level_dBm;
        public double? AntennaPatternLoss_dB;
        public double? diffractionLoss_dB;
    }
}
