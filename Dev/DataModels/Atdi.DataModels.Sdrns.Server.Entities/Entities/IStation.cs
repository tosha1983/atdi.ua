using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IStation
    {
        int Id { get; set; }
        string GlobalSID { get; set; }
        string Status { get; set; }
        string Standart { get; set; }
        int? MeasTaskId { get; set; }
        int? StationId { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        DateTime CloseDate { get; set; }
        string DozvilName { get; set; }
        int? OwnerDataId { get; set; }
        int? StationSiteId { get; set; }
        IMeasTask MEASTASK { get; set; }
        IStationSite STATIONSITE { get; set; }
        IOwnerData OWNERDATA { get; set; }
    }
}
