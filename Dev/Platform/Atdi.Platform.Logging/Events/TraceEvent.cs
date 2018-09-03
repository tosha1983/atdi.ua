using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    class TraceEvent : DebugEvent, ITraceEvent
    {
        public TraceEvent(TraceEventType eventType) : base(EventLevel.Trace)
        {
            this.EventType = eventType;
        }

        public TraceEventType EventType { get; set; }

        public TimeSpan? Duration { get; set; }

        public ITraceScopeData ScopeData { get; set; }
    }
}
