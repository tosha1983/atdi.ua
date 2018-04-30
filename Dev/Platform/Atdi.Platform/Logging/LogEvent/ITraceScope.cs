using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    public interface ITraceScope : IDisposable
    {
        TimeSpan? CurrentDuration { get; }

        TimeSpan? LastDuration { get; }

        IBeginTraceEvent BeginEvent { get; }

        void SetData<T>(string key, T value);

        void Trace(EventText eventText, string source, IReadOnlyDictionary<string, object> data);

        ITraceScopeData ScopeData { get; }
    }

    public static class LogEventTraceScopeExtension
    {
        public static void Trace(this ITraceScope scope, EventText eventText)
        {
            scope.Trace(eventText, null, null);
        }

        public static void Trace(this ITraceScope scope, EventText eventText, IReadOnlyDictionary<string, object> data)
        {
            scope.Trace(eventText, null, data);
        }
        public static void Trace(this ITraceScope scope, EventText eventText, string source)
        {
            scope.Trace(eventText, source, null);
        }
    }
}
