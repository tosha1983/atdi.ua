using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers
{
    public class AppServerComponentConfig
    {
        [ComponentConfigProperty("ResSysInfo.Data")]
        public bool? ResSysInfoData { get; set; }
    }
}
