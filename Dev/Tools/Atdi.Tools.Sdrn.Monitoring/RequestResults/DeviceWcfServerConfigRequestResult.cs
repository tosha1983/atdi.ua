using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Tools.Sdrn.Monitoring
{
    public class DeviceWcfServerConfigRequestResult
    {
        public string Instance { get; set; }
        public string SdrnServerInstance { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime LicenseStopDate { get; set; }
        public DateTime LicenseStartDate { get; set; }
        public Dictionary<string, string> AllowedSensors { get; set; }
    }
}
