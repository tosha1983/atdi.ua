using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    class ExceptionData : IExceptionData
    {
        public ExceptionData(Exception e)
        {
            this.Type = e.GetType().Name;
            this.Source = e.Source;
            this.StackTrace = e.StackTrace;
            this.TargetSite = e.TargetSite == null ? string.Empty : e.TargetSite.Name;
            this.Message = e.Message;

            if (e.InnerException != null)
            {
                this.Inner = new ExceptionData(e.InnerException);
            }
        }

        public string Type { get; private set; }

        public string Source { get; private set; }

        public string StackTrace { get; private set; }

        public string TargetSite { get; private set; }

        public IExceptionData Inner { get; private set; }

        public string Message { get; private set; }
    }
}
