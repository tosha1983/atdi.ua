using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    class DebugEvent : Event, IDebugEvent
    {
        public DebugEvent(EventLevel level) : base(level)
        {
        }

        public string Source { get; set; }

        public IReadOnlyDictionary<string, string> Data { get; set; }
    }
}
