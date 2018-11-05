using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ISector
    {
        int Id { get; set; }
        double? Agl { get; set; }
        double? Eirp { get; set; }
        double? Azimut { get; set; }
        double? Bw { get; set; }
        string ClassEmission { get; set; }
        int? StationId { get; set; }
        int? SectorId { get; set; }
        IStation STATION { get; set; }
    }
}
