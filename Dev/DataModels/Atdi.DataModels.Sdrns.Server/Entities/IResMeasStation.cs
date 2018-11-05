using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IResMeasStation
    {
        int Id { get; set; }
        string GlobalSID { get; set; }
        string MeasGlobalSID { get; set; }
        int? SectorId { get; set; }
        int? IdStation { get; set; }
        string Status { get; set; }
        int? ResMeasId { get; set; }
        string Standard { get; set; }
        int? StationId { get; set; }
        ISector SECTOR { get; set; }
        IResMeas RESMEAS { get; set; }
        IStation STATION { get; set; }
    }
}
