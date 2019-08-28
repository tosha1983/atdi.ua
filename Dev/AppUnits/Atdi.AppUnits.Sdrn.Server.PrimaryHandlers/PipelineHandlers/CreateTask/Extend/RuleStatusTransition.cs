using System;
using System.Collections.Generic;
using System.Linq;


namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.PipelineHandlers
{
    public class RuleStatusTransition
    {
        public string NameOperation { get; set; }
        public string StartStatus { get; set; }
        public string ToStatuses { get; set; }
        public RuleStatusTransition(string NameOperation_, string StartStatus_, string ToStatuses_)
        {
            NameOperation = NameOperation_;
            StartStatus = StartStatus_;
            ToStatuses = ToStatuses_;
        }
    }
}
