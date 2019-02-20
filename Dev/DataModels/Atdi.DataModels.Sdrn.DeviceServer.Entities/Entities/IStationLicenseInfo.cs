using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [Entity]
    public interface IStationLicenseInfo
    {
        int Id { get; set; }
        int? IcsmId { get; set; }
        string Name { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
        DateTime? CloseDate { get; set; }
    }
}
