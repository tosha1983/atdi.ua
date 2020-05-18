using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.EntityOrm.WebClient;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient
{
	public class AppComponentConfig
	{
		public string Instance { get; set; }

		[ComponentConfigProperty("CalcServer.EntityOrm.Endpoint.BaseAddress")]
		public string CalcServerEntityOrmEndpointBaseAddress { get; set; }

		[ComponentConfigProperty("CalcServer.EntityOrm.Endpoint.ApiUri")]
		public string CalcServerEntityOrmEndpointApiUri { get; set; }

		[ComponentConfigProperty("CalcServer.EntityOrm.DataContext")]
		public string CalcServerEntityOrmDataContext { get; set; }


		[ComponentConfigProperty("Infocenter.EntityOrm.Endpoint.BaseAddress")]
		public string InfocenterEntityOrmEndpointBaseAddress { get; set; }

		[ComponentConfigProperty("Infocenter.EntityOrm.Endpoint.ApiUri")]
		public string InfocenterEntityOrmEndpointApiUri { get; set; }

		[ComponentConfigProperty("Infocenter.EntityOrm.DataContext")]
		public string InfocenterEntityOrmDataContext { get; set; }



	}
}
