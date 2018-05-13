using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    class EndTraceEvent : TraceEvent, IEndTraceEvent
    {
        public EndTraceEvent(ITraceScope scope) : base(TraceEventType.EndScope)
        {
            this.BeginEvent = scope.BeginEvent;
        }

        public IBeginTraceEvent BeginEvent { get; private set; }
    }
}
