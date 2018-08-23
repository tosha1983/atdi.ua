using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;

namespace Atdi.WcfServices.Sdrn.Device
{
    public class SdrnDeviceComponent : WcfServicesComponent
    {
        public SdrnDeviceComponent() : base("SdrnDeviceServices", ComponentBehavior.SingleInstance)
        {
        }
    }
}
