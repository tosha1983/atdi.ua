using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IStation_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IStation : IStation_PK
    {
        string GlobalSID { get; set; }
        string Status { get; set; }
        string Standart { get; set; }
        long? MeasTaskId { get; set; }
        long? StationId { get; set; }
        int? IdPermission { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        DateTime CloseDate { get; set; }
        string DozvilName { get; set; }
        long? OwnerDataId { get; set; }
        long? StationSiteId { get; set; }
        IMeasTask MEASTASK { get; set; }
        IStationSite STATIONSITE { get; set; }
        IOwnerData OWNERDATA { get; set; }
    }
}
