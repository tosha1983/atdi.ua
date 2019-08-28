using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Events
{
    public class SOMeasResultAggregated : Event
    {
        public SOMeasResultAggregated() : base()
        {
        }
        public SOMeasResultAggregated(string name, string source) : base(name, source)
        {
        }
        public long MeasResultId { get; set; }
    }
}
