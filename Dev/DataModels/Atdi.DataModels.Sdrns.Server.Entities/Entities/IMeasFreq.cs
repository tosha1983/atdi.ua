using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IMeasFreq_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IMeasFreq: IMeasFreq_PK
    {
        double? Freq { get; set; }
        long? MeasFreqParamId { get; set; }
        IMeasFreqParam MEASFREQPARAM { get; set; }
    }
}
