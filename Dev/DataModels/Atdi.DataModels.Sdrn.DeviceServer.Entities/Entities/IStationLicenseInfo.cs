using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IStationLicenseInfo_PK
    {
        long? Id { get; set; }
    }

    [Entity]
    public interface IStationLicenseInfo : IStationLicenseInfo_PK
    {
        int? IcsmId { get; set; }
        string Name { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
        DateTime? CloseDate { get; set; }
    }
}
