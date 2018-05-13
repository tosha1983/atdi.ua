using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    public interface ITraceEvent : IDebugEvent
    {
        TraceEventType EventType { get; }

        TimeSpan? Duration { get; }

        ITraceScopeData ScopeData { get; }
    }
}
