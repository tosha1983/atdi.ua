using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Atdi.Contracts.Sdrn.Infocenter;
using Atdi.Platform.Logging;

namespace Atdi.WebApiServices.Sdrn.Infocenter.Controllers
{
	[RoutePrefix("api/Infocenter")]
	public class InfocenterController : WebApiController
	{
		private readonly IInfocenterConfig _serverConfig;

		public InfocenterController(IInfocenterConfig serverConfig, ILogger logger)
			: base(logger)
		{
			this._serverConfig = serverConfig;
		}

		[HttpGet]
		[Route("config")]
		public IInfocenterConfig Config()
		{
			return this._serverConfig;
		}
	}
}
