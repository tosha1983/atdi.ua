using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WebApiServices.Sdrn.DeviceServer
{

    public sealed class AppServerComponent : WebApiServicesComponent
    {
        public AppServerComponent() : base("SdrnDeviceServerWebApiServices", ComponentBehavior.Simple)
        {
        }
    }
}
