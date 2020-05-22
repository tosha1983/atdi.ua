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
        public string GlobalSIDByICSM { get; set; }
        public string GlobalSIDByMeasurement { get; set; }
        public string CodeRegion { get; set; }
        public long? StationIdByICSM { get; set; }
        public string TableNameByICSM { get; set; }
        public StationStatus Status { get; set; }
        public string NameGroupGlobalSID { get; set; }
    }
}
