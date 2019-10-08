using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;

namespace Atdi.AppUnits.Icsm.CoverageEstimation
{
    public sealed class AppServerComponentConfig
    {
        [ComponentConfigProperty("Telecom.TargetFolder")]
        public string TelecomTargetFolder { get; set; }
    }
}
