using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ISectorFreq_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ISectorFreq : ISectorFreq_PK
    {
        long? ClientPlanCode { get; set; }
        long? ChannelNumber { get; set; }
        long? ClientFreqCode { get; set; }
        decimal? Frequency { get; set; }
    }
}
