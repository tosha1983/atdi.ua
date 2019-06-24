using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IMeasStation_PK
    {
        long? Id { get; set; }
    }
        [Entity]
    public interface IMeasStation : IMeasStation_PK
    {
        string StationId { get; set; }
        string GlobalSid { get; set; }
        string OwnerGlobalSid { get; set; }
        string Status { get; set; }
        string Standard { get; set; }
        long? MeasTaskId { get; set; }
        long? OwnerId { get; set; }
        long? SiteId { get; set; }
        long? LicenceId { get; set; }
        IMeasTask MEASTASK { get; set; }
        IOwnerData OWNER { get; set; }
        IStationSite SITE { get; set; }
        IStationLicenseInfo LICENCE { get; set; }
    }
}
