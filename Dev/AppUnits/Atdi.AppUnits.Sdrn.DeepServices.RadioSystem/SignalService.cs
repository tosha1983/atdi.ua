using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;
using Atdi.Platform.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;

namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem
{
	public class SignalService : ISignalService
	{

		public double CalcAntennaGain(in CalcAntennaGainArgs args)
		{
            return Signal.CalcAntennaGain.Calc(in args);
		}

		public void CalcLoss(in CalcLossArgs args, ref CalcLossResult result)
		{
            result = Signal.PropagationLoss.Calc(args);
		}

		private void CalcMainBlock(in MainCalcBlock block)
		{

		}

		public void Dispose()
		{
			
		}
	}
}
