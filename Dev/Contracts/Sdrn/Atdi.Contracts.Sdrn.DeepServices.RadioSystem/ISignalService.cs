using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeepServices.RadioSystem
{
	public interface ISignalService : IDeepService
	{
		/// <summary>
		/// 
		/// </summary>
		void CalcLoss(in CalcLossArgs args, ref CalcLossResult result);


		float CalcAntennaGain(in CalcAntennaGainArgs args);

	}
}
