using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers
{
    public class Configs
    {
        [ComponentConfigProperty("ResSysInfo.Data")]
        public bool? ResSysInfoData { get; set; }
    }
}
