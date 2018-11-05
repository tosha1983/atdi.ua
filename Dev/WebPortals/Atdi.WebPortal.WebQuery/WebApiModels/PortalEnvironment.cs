using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    public class PortalEnvironment
    {
        public string Title { get; set; }

        public string Version { get; set; }

        public PortalCompany Company { get; set; }

        public PortalUser User { get; set; }
    }
}
