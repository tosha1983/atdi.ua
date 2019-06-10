﻿using Atdi.DataModels.EntityOrm;
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
        long Id { get; set; }
        double? Freq { get; set; }
        long? MeasFreqParamId { get; set; }
        IMeasFreqParam MEASFREQPARAM { get; set; }
    }
}
