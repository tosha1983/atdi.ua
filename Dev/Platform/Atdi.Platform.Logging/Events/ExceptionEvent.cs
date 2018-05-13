using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    class ExceptionEvent : DebugEvent, IExceptionEvent
    {
        public ExceptionEvent(Exception e) : base(EventLevel.Exception)
        {
            if (e == null)
            {
                throw new ArgumentNullException();
            }
            this.Exception = new ExceptionData(e);
        }

        public IExceptionData Exception { get; set; }
    }
}
