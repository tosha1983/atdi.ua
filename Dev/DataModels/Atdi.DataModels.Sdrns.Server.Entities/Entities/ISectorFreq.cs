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
        long Id { get; set; }
        long? PlanId { get; set; }
        long? ChannelNumber { get; set; }
        long? IdFreq { get; set; }
        decimal? Frequency { get; set; }
    }
}
