using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ISectorFreq_PK
    {
        long? Id { get; set; }
    }

    [Entity]
    public interface ISectorFreq : ISectorFreq_PK
    {
        long? PlanId { get; set; }
        long? ChannelNumber { get; set; }
        double? Frequency { get; set; }
        long? IdSector { get; set; }
    }
}
