using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery
{
    public class PortalSettings
    {
        public string WebQueryApiUrl { get; set; }

        public string Title { get; set; }

        public string Version { get; set; }
        
        public string CompanyTitle { get; set; }

        public string CompanySite { get; set; }

        public string CompanyEmail { get; set; }

        public string LicenseFileName { get; set; }

        public string LicenseOwnerId { get; set; }

        public string LicenseProductKey { get; set; }
    }
}
