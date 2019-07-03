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
        long? IdStation{ get; set; }
        string GlobalSID { get; set; }
        string Status { get; set; }
        string Standart { get; set; }
        int? IdPermission { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        DateTime CloseDate { get; set; }
        string DozvilName { get; set; }
        IMeasTask MEAS_TASK { get; set; }
        IStationSite STATION_SITE { get; set; }
        IOwnerData OWNER_DATA { get; set; }
    }
}
