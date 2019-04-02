
using Atdi.Contracts.Sdrn.Server;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Atdi.WebApiServices.Sdrn.Server.Controllers
{
    public class SdrnServerController : WebApiController
    {
        private readonly ISdrnServerEnvironment _serverEnvironment;

        public SdrnServerController(ISdrnServerEnvironment serverEnvironment, ILogger logger) : base(logger)
        {
            this._serverEnvironment = serverEnvironment;
        }

        [HttpGet]
        public ISdrnServerEnvironment Config()
        {
            return this._serverEnvironment;
        }
    }
}
