using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ISectorFreq
    {
        int Id { get; set; }
        int? PlanId { get; set; }
        int? ChannelNumber { get; set; }
        double? Frequency { get; set; }
    }
}
