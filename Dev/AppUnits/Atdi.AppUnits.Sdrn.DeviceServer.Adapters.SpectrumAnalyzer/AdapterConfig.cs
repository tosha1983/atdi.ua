using Atdi.Platform.AppComponent;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SpectrumAnalyzer
{
    public class AdapterConfig
    {
        public string SerialNumber { get; set; }
        public string IPAddress { get; set; }
        public int ConnectionMode { get; set; }
        public bool DisplayUpdate { get; set; }
        public bool OnlyAutoSweepTime { get; set; }
        public int Optimization { get; set; }

    }
}
