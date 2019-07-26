
using Atdi.Contracts.WcfServices.Sdrn.Device;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Atdi.WebApiServices.Sdrn.DeviceWcfService.Controllers
{
    [RoutePrefix("api/SdrnDeviceWcfService")]
    public class ServiceController : WebApiController
    {

        private readonly IServiceEnvironment _serviceEnvironment;

        public ServiceController(IServiceEnvironment serviceEnvironment, ILogger logger) : base(logger)
        {
            this._serviceEnvironment = serviceEnvironment;
        }

        [HttpGet]
        [Route("config")]
        public IServiceEnvironment Config()
        {
            return this._serviceEnvironment;
        }
    }
}
