using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IMeasFreq
    {
        int Id { get; set; }
        double? Freq { get; set; }
        int MeasFreqParamId { get; set; }
        IMeasFreqParam MEASFREQPARAM { get; set; }
    }
}
