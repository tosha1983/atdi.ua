using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.Identity
{
	public interface IAuthServiceSite
	{
		void Registry(string providerName, Type providerType);

		IAuthService GetService(string serviceName, Type providerType);

	}
}
