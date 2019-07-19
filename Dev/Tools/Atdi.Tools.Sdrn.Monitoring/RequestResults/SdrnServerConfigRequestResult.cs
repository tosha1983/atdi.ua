using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Tools.Sdrn.Monitoring
{
    public class SdrnServerConfigRequestResult
    {
        public string ServerInstance { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime LicenseStartDate { get; set; }
        public DateTime LicenseStopDate { get; set; }
        public string ServerRoles { get; set; }
        public string MasterServerInstance { get; set; }
    }
}
