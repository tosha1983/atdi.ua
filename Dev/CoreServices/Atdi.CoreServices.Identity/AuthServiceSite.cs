using Atdi.Contracts.CoreServices.Identity;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.DependencyInjection;

namespace Atdi.CoreServices.Identity
{
	public sealed class AuthServiceSite : IAuthServiceSite
	{
		private readonly IServicesResolver _resolver;
		private readonly ConcurrentDictionary<string, Type> _serviceTypes;

		public AuthServiceSite(IServicesContainer servicesContainer)
		{
			this._resolver = servicesContainer.GetResolver<IServicesResolver>();
			_serviceTypes = new ConcurrentDictionary<string, Type>();
		}
		public IAuthService GetService(string serviceName)
		{
			if (_serviceTypes.TryGetValue(serviceName, out var type))
			{
				return _resolver.Resolve(type) as IAuthService;
			}

			return null;
		}

		public void Register(string serviceName, Type serviceType)
		{
			_serviceTypes.TryAdd(serviceName, serviceType);
		}
	}
}
