using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    class CriticalEvent : DebugEvent, ICriticalEvent
    {
        public CriticalEvent(Exception e = null) : base(EventLevel.Critical)
        {
            if (e != null)
            {
                this.Exception = new ExceptionData(e);
            }
        }

        public IExceptionData Exception { get; set; }
    }
}
