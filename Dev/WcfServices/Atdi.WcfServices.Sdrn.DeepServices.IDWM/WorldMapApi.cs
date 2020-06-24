using Atdi.Contracts.WcfServices.Sdrn.DeepServices.IDWM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WcfServices.Sdrn.DeepServices.IDWM
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class WorldMapApi : WcfServiceBase<IWorldMapApi>, IWorldMapApi
	{

		public int Calc1(double a, double b)
		{
			throw new NotImplementedException();
		}

		public int Calc2(double a, double b)
		{
			throw new NotImplementedException();
		}

		public int Calc3(double a, double b)
		{
			throw new NotImplementedException();
		}
	}
}
