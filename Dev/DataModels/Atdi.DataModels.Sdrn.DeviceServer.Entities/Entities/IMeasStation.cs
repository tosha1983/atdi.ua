using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [Entity]
    public interface IMeasStation
    {
        int Id { get; set; }
        string StationId { get; set; }
        string GlobalSid { get; set; }
        string OwnerGlobalSid { get; set; }
        string Status { get; set; }
        string Standard { get; set; }
        int? MeasTaskId { get; set; }
        int? OwnerId { get; set; }
        int? SiteId { get; set; }
        int? LicenceId { get; set; }
        IMeasTask MEASTASK { get; set; }
        IOwnerData OWNER { get; set; }
        IStationSite SITE { get; set; }
        IStationLicenseInfo LICENCE { get; set; }
    }
}
