using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.UnitTest.AppUnits.Sdrn.Server.PrimaryHandlers.Fake
{
    class FakeLogger : ILogger
    {
        class FakeTraceScope : ITraceScope
        {
            public TimeSpan? CurrentDuration { get;  }

            public TimeSpan? LastDuration { get; }

            public IBeginTraceEvent BeginEvent { get; }

            public ITraceScopeData ScopeData { get; }

            public void Dispose()
            {
                
            }

            public void SetData<T>(string key, T value)
            {
                
            }

            public void Trace(EventText eventText, string source, IReadOnlyDictionary<string, object> data)
            {
                
            }
        }
        public void Critical(EventContext context, EventCategory category, EventText eventText, Exception e, string source, IReadOnlyDictionary<string, object> data)
        {
            
        }

        public void Debug(EventContext context, EventCategory category, EventText eventText, string source, IReadOnlyDictionary<string, object> data)
        {
            
        }

        public void Dispose()
        {
            
        }

        public void Error(EventContext context, EventCategory category, EventText eventText, string source, IReadOnlyDictionary<string, object> data)
        {
            
        }

        public void Exception(EventContext context, EventCategory category, EventText eventText, Exception e, string source, IReadOnlyDictionary<string, object> data)
        {
            
        }

        public void Info(EventContext context, EventCategory category, EventText eventText)
        {
            
        }

        public bool IsAllowed(EventLevel level)
        {
            return true;
        }

        public ITraceScope StartTrace(EventContext context, EventCategory category, TraceScopeName scopeName, string source, IReadOnlyDictionary<string, object> data)
        {
            return new FakeTraceScope();
        }

        public void Verbouse(EventContext context, EventCategory category, EventText eventText)
        {
            
        }

        public void Warning(EventContext context, EventCategory category, EventText eventText)
        {
            
        }
    }
}
