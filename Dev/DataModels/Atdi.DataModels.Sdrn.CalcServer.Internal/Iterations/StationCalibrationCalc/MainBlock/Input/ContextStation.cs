using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
    public class ContextStation : ClientContextStation
    {
        public DateTimeOffset? ModifiedDate;
        public string LicenseGsid { get; set; }
        public string RealGsid { get; set; }
        public string RegionCode { get; set; }
        public string ExternalCode { get; set; }
        public string ExternalSource { get; set; }
        public string NameGroupGlobalSID { get; set; }
        public string RealStandard { get; set; }
}
}
