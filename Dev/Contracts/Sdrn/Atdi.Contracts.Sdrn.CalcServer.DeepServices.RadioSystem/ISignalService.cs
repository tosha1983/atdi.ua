using Atdi.DataModels.Sdrn.CalcServer.DeepServices.RadioSystem.SignalService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.CalcServer.DeepServices.RadioSystem
{
	public interface ISignalService : IDeepService
	{
		/// <summary>
		/// 
		/// </summary>
		void CalcLoss(ref CalcLossArgs args, ref CalcLossResult result);

		void CalcAntennaGain();

		
	}
}
