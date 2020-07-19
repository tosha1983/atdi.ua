using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.Identity
{
	public class ExternalService
	{
		public string Id;

		public string Name;

		/// <summary>
		/// Секретный ключ
		/// </summary>
		public string SecretKey;
	}

	public interface IExternalServiceProvider
	{
		void Register(ExternalService service);

		ExternalService GetServiceById(string sid);
	}
}
