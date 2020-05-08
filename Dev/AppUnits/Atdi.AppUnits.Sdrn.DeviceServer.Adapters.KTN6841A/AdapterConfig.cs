using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.KTN6841A
{
    public class AdapterConfig
    {
        public string SmsHostName { get; set; } = "";
        public string SensorName { get; set; } = "";
        /// <summary>
        /// Hann     = 1    
        /// Gausstop = 2     
        /// Flattop  = 4     
        /// Uniform  = 8     
        /// </summary>
        public int WindowType { get; set; } = 0;
        public bool UseGNSS { get; set; } = false;
    }
}
