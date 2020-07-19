using Atdi.Contracts.CoreServices.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.Identity
{
	public sealed class ExternalServiceProvider : IExternalServiceProvider
	{
		private readonly Dictionary<string, ExternalService> _externalServices;

		public ExternalServiceProvider()
		{
			this._externalServices = new Dictionary<string, ExternalService>();
		}

		public ExternalService GetServiceById(string sid)
		{
			_externalServices.TryGetValue(sid, out var result);
			return result;
		}

		public void Register(ExternalService service)
		{
			if (_externalServices.ContainsKey(service.Id))
			{
				throw new InvalidOperationException($"Service with ID #{service.Id} was registered earlier");
			}
			_externalServices.Add(service.Id, service);
		}
	}
}
