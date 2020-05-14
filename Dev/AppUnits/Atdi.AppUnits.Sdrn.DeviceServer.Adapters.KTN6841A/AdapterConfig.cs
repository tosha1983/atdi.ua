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
        public bool SensorInLocalNetwork { get; set; } = false;
        /// <summary>
        /// Hann     = 1    
        /// Gausstop = 2     
        /// Flattop  = 4     
        /// Uniform  = 8     
        /// </summary>
        public uint WindowType { get; set; } = 0;
        public bool UseGNSS { get; set; } = false;
        public bool LockSensorResource { get; set; } = false;
        public uint SelectedAntenna { get; set; } = 0;
    }
}
