using Atdi.Platform.AppComponent;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound
{
    public class AdapterConfig
    {        
        public string SerialNumber { get; set; }

        public bool Reference10MHzConnected { get; set; }
        public bool GPSPPSConnected { get; set; }
        public bool SyncCPUtoGPS { get; set; }
        public int GPSPortNumber { get; set; }
        public int GPSPortBaudRate { get; set; }
    }
}
