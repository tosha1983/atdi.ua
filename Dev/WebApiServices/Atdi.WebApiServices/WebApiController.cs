using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Atdi.WebApiServices
{
    public abstract class WebApiController : ApiController
    {
        private readonly ILogger _logger;

        public WebApiController(ILogger logger)
        {
            this._logger = logger;
        }
    }
}
