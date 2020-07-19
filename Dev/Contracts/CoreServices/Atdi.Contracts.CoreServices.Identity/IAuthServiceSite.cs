using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.Identity
{
	public interface IAuthServiceSite
	{
		void Register(string serviceName, Type serviceType);

		IAuthService GetService(string serviceName);

	}
}
