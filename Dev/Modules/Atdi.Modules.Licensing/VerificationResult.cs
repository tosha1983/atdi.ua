using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Licensing
{
    public class VerificationResult
    {
        public string LicenseNumber { get; set; }

        public string OwnerName { get; set; }

        public DateTime StopDate { get; set; }

        public int Count { get; set; }

        public string Instance { get; set; }

        public DateTime StartDate { get; set; }

        public ExternalServiceDescriptor[] ExternalServices { get; set; }
	}
}
