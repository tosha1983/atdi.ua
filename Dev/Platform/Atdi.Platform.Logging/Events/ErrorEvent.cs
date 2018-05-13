using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    class ErrorEvent : DebugEvent, IErrorEvent
    {
        public ErrorEvent() : base(EventLevel.Error)
        {
        }

    }
}
