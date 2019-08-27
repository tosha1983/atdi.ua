using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Events
{
    public class SOMeasResultAppeared : Event
    {
        public SOMeasResultAppeared() : base()
        {
        }
        public SOMeasResultAppeared(string name, string source) : base(name, source)
        {
        }
        public long MeasResultId { get; set; }
    }
}
