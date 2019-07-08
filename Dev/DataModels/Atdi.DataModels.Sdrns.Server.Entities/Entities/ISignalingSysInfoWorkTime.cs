using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ISignalingSysInfoWorkTime_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ISignalingSysInfoWorkTime : ISignalingSysInfoWorkTime_PK
    {
        DateTime? StartEmitting { get; set; }
        DateTime? StopEmitting { get; set; }
        int? HitCount { get; set; }
        float? PersentAvailability { get; set; }
        ISignalingSysInfo SIGN_SYSINFOS { get; set; }
    }
}
