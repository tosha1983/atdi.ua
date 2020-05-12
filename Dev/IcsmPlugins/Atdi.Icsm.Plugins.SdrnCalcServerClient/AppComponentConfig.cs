using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient
{
	public class AppComponentConfig
	{
		[ComponentConfigProperty("EntityOrmClient.Endpoint.BaseAddress")]
		public string OrmEndpointBaseAddress { get; set; }

		[ComponentConfigProperty("EntityOrmClient.Endpoint.ApiUri")]
		public string OrmEndpointApiUri { get; set; }
	}
}
