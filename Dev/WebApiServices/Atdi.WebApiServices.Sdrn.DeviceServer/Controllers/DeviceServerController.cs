using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Atdi.WebApiServices.Sdrn.DeviceServer.Controllers
{
    public class DeviceServerController : WebApiController
    {
        private readonly IDeviceServerConfig _deviceServerConfig;

        public DeviceServerController(IDeviceServerConfig deviceServerConfig, ILogger logger) : base(logger)
        {
            this._deviceServerConfig = deviceServerConfig;
        }

        [HttpGet]
        public IDeviceServerConfig Config()
        {
            return this._deviceServerConfig;
        }
    }
}
