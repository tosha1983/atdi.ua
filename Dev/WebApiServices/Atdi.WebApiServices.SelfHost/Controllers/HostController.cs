using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Atdi.Platform.AppServer;
using Atdi.Platform.Logging;
using Atdi.WebApiServices.SelfHost.Controllers.Models;

namespace Atdi.WebApiServices.SelfHost.Controllers
{
    public class HostController : WebApiController
    {
        private readonly IServerContext _context;

        public HostController(IServerContext context, ILogger logger) : base(logger)
        {
            this._context = context;
        }

        [HttpGet]
        public HostInfo Info()
        {
            return new HostInfo
            {
                Instance = _context.Config.Instance,
                Components = _context.Config.Components
            };
        }
    }
}
