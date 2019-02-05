using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.UnitTest.Sdrn.DeviceServer
{
    public class ConsoleLogger : ILogger
    {
        private class TraceScope : ITraceScope
        {
            public TimeSpan? CurrentDuration => null;

            public TimeSpan? LastDuration => null;

            public IBeginTraceEvent BeginEvent => null;

            public ITraceScopeData ScopeData => null;

            public void Dispose()
            {
                
            }

            public void SetData<T>(string key, T value)
            {
               
            }

            public void Trace(EventText eventText, string source, IReadOnlyDictionary<string, object> data)
            {
                Console.WriteLine($"[Trace]: '{eventText}' ({source})");
            }
        }

        private void Write(string level, EventContext context, EventCategory category, EventText eventText, Exception e, string source, IReadOnlyDictionary<string, object> data)
        {
            Console.WriteLine($"[{level}][{context}][{category}]: '{eventText}' ({e?.Message})({source})");
        }

        public void Critical(EventContext context, EventCategory category, EventText eventText, Exception e, string source, IReadOnlyDictionary<string, object> data)
        {
            this.Write("Critical", context, category, eventText, e, source, data);
        }

        public void Debug(EventContext context, EventCategory category, EventText eventText, string source, IReadOnlyDictionary<string, object> data)
        {
            this.Write("Debug", context, category, eventText, null, source, data);
        }

        public void Dispose()
        {
           
        }

        public void Error(EventContext context, EventCategory category, EventText eventText, string source, IReadOnlyDictionary<string, object> data)
        {
            this.Write("Debug", context, category, eventText, null, source, data);
        }

        public void Exception(EventContext context, EventCategory category, EventText eventText, Exception e, string source, IReadOnlyDictionary<string, object> data)
        {
            this.Write("Debug", context, category, eventText, null, source, data);
        }

        public void Info(EventContext context, EventCategory category, EventText eventText)
        {
            this.Write("Debug", context, category, eventText, null, null, null);
        }

        public bool IsAllowed(EventLevel level)
        {
            return true;
        }

        public ITraceScope StartTrace(EventContext context, EventCategory category, TraceScopeName scopeName, string source, IReadOnlyDictionary<string, object> data)
        {
            this.Write("StartTrace", context, category, (EventText)(scopeName.Name), null, source, data);
            return new TraceScope();
        }

        public void Verbouse(EventContext context, EventCategory category, EventText eventText)
        {
            this.Write("Verbouse", context, category, eventText, null, null, null);
        }

        public void Warning(EventContext context, EventCategory category, EventText eventText)
        {
            this.Write("Warning", context, category, eventText, null, null, null);
        }
    }
}
