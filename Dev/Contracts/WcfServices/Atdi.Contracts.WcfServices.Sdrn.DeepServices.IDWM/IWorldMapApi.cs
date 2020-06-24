using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.WcfServices.Sdrn.DeepServices.IDWM
{
	[ServiceContract(Namespace = "http://schemas.atdi.com/appserver/services/sdrn/deepservices/idwn")]
	public interface IWorldMapApi
	{
		[OperationContract]
		int Calc1(double a, double b);

		[OperationContract]
		int Calc2(double a, double b);

		[OperationContract]
		int Calc3(double a, double b);
	}
}
