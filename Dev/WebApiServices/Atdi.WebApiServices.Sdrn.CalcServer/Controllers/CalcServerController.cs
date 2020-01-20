using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Platform.Logging;

namespace Atdi.WebApiServices.Sdrn.CalcServer.Controllers
{
	[RoutePrefix("api/SdrnCalcServer")]
	public class CalcServerController : WebApiController
	{
		private readonly ICalcServerConfig _serverConfig;

		public CalcServerController(ICalcServerConfig serverConfig, ILogger logger)
			: base(logger)
		{
			this._serverConfig = serverConfig;
		}

		[HttpGet]
		[Route("config")]
		public ICalcServerConfig Config()
		{
			return this._serverConfig;
		}
	}
}
