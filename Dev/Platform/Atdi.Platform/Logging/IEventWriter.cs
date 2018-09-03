using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    public interface IEventWriter
    {
        void Verbouse(EventContext context, EventCategory category, EventText eventText);

        void Info(EventContext context, EventCategory category, EventText eventText);

        void Warning(EventContext context, EventCategory category, EventText eventText);

        void Debug(EventContext context, EventCategory category, EventText eventText, string source, IReadOnlyDictionary<string, object> data);

        ITraceScope StartTrace(EventContext context, EventCategory category, TraceScopeName scopeName, string source, IReadOnlyDictionary<string, object> data);

        void Error(EventContext context, EventCategory category, EventText eventText, string source, IReadOnlyDictionary<string, object> data);

        void Exception(EventContext context, EventCategory category, EventText eventText, Exception e, string source, IReadOnlyDictionary<string, object> data);

        void Critical(EventContext context, EventCategory category, EventText eventText, Exception e, string source, IReadOnlyDictionary<string, object> data);
    }

    public static class LogWriterExtension
    {
        #region Verbouse-Info-Warning
        public static void Verbouse(this IEventWriter writer, EventContext context, EventText eventText)
        {
            writer.Verbouse(context, null, eventText);
        }

        public static void Info(this IEventWriter writer, EventContext context, EventText eventText)
        {
             writer.Info(context, null, eventText);
        }

        public static void Warning(this IEventWriter writer, EventContext context, EventText eventText)
        {
            writer.Warning(context, null, eventText);
        }

        #endregion
        
        #region Debug

        public static void Debug(this IEventWriter writer, EventContext context, EventText eventText)
        {
            writer.Debug(context, null, eventText, null, null);
        }
        public static void Debug(this IEventWriter writer, EventContext context, EventText eventText, string source)
        {
            writer.Debug(context, null, eventText, source, null);
        }
        public static void Debug(this IEventWriter writer, EventContext context, EventText eventText, IReadOnlyDictionary<string, object> data)
        {
            writer.Debug(context, null, eventText, null, data);
        }
        public static void Debug(this IEventWriter writer, EventContext context, EventText eventText, string source, IReadOnlyDictionary<string, object> data)
        {
            writer.Debug(context, null, eventText, source, data);
        }

        public static void Debug(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText)
        {
            writer.Debug(context, category, eventText, null, null);
        }
        public static void Debug(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText, string source)
        {
            writer.Debug(context, category, eventText, source, null);
        }
        public static void Debug(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText, IReadOnlyDictionary<string, object> data)
        {
            writer.Debug(context, category, eventText, null, data);
        }

        #endregion

        #region StartTrace

        public static ITraceScope StartTrace(this IEventWriter writer, EventContext context, TraceScopeName scopeName)
        {
            return writer.StartTrace(context, null, scopeName, null, null);
        }
        public static ITraceScope StartTrace(this IEventWriter writer, EventContext context, TraceScopeName scopeName, string source)
        {
            return writer.StartTrace(context, null, scopeName, source, null);
        }
        public static ITraceScope StartTrace(this IEventWriter writer, EventContext context, TraceScopeName scopeName, IReadOnlyDictionary<string, object> data)
        {
            return writer.StartTrace(context, null, scopeName, null, data);
        }
        public static ITraceScope StartTrace(this IEventWriter writer, EventContext context, TraceScopeName scopeName, string source, IReadOnlyDictionary<string, object> data)
        {
            return writer.StartTrace(context, null, scopeName, source, data);
        }


        public static ITraceScope StartTrace(this IEventWriter writer, EventContext context, string source)
        {
            return writer.StartTrace(context, null, null, source, null);
        }

        public static ITraceScope StartTrace(this IEventWriter writer, EventContext context, string source, IReadOnlyDictionary<string, object> data)
        {
            return writer.StartTrace(context, null, null, source, data);
        }

        public static ITraceScope StartTrace(this IEventWriter writer, EventContext context, EventCategory category, TraceScopeName scopeName)
        {
            return writer.StartTrace(context, category, scopeName, null, null);
        }
        public static ITraceScope StartTrace(this IEventWriter writer, EventContext context, EventCategory category, TraceScopeName scopeName, string source)
        {
            return writer.StartTrace(context, category, scopeName, source, null);
        }
        public static ITraceScope StartTrace(this IEventWriter writer, EventContext context, EventCategory category, TraceScopeName scopeName, IReadOnlyDictionary<string, object> data)
        {
            return writer.StartTrace(context, category, scopeName, null, data);
        }

        public static ITraceScope StartTrace(this IEventWriter writer, EventContext context, EventCategory category, string source)
        {
            return writer.StartTrace(context, category, null, source, null);
        }
        public static ITraceScope StartTrace(this IEventWriter writer, EventContext context, EventCategory category, string source, IReadOnlyDictionary<string, object> data)
        {
            return writer.StartTrace(context, category, null, source, data);
        }

        #endregion

        #region Error

        public static void Error(this IEventWriter writer, EventContext context, EventText eventText)
        {
            writer.Error(context, null, eventText, null, null);
        }
        public static void Error(this IEventWriter writer, EventContext context, EventText eventText, string source)
        {
            writer.Error(context, null, eventText, source, null);
        }
        public static void Error(this IEventWriter writer, EventContext context, EventText eventText, IReadOnlyDictionary<string, object> data)
        {
            writer.Error(context, null, eventText, null, data);
        }
        public static void Error(this IEventWriter writer, EventContext context, EventText eventText, string source, IReadOnlyDictionary<string, object> data)
        {
            writer.Error(context, null, eventText, source, data);
        }


        public static void Error(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText)
        {
            writer.Error(context, category, eventText, null, null);
        }
        public static void Error(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText, string source)
        {
            writer.Error(context, category, eventText, source, null);
        }
        public static void Error(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText, IReadOnlyDictionary<string, object> data)
        {
            writer.Error(context, category, eventText, null, data);
        }

        #endregion

        #region Exception

        public static void Exception(this IEventWriter writer, EventContext context, Exception e)
        {
            writer.Exception(context, null, null, e, null, null);
        }
        public static void Exception(this IEventWriter writer, EventContext context, Exception e, string source)
        {
            writer.Exception(context, null, null, e, source, null);
        }
        public static void Exception(this IEventWriter writer, EventContext context, Exception e, IReadOnlyDictionary<string, object> data)
        {
            writer.Exception(context, null, null, e, null, data);
        }
        public static void Exception(this IEventWriter writer, EventContext context, Exception e, string source, IReadOnlyDictionary<string, object> data)
        {
            writer.Exception(context, null, null, e, source, data);
        }

        public static void Exception(this IEventWriter writer, EventContext context, EventCategory category, Exception e)
        {
            writer.Exception(context, category, null, e, null, null);
        }
        public static void Exception(this IEventWriter writer, EventContext context, EventCategory category, Exception e, string source)
        {
            writer.Exception(context, category, null, e, source, null);
        }
        public static void Exception(this IEventWriter writer, EventContext context, EventCategory category, Exception e, IReadOnlyDictionary<string, object> data)
        {
            writer.Exception(context, category, null, e, null, data);
        }
        public static void Exception(this IEventWriter writer, EventContext context, EventCategory category, Exception e, string source, IReadOnlyDictionary<string, object> data)
        {
            writer.Exception(context, category, null, e, source, data);
        }


        public static void Exception(this IEventWriter writer, EventContext context, EventText eventText, Exception e)
        {
            writer.Exception(context, null, eventText, e, null, null);
        }
        public static void Exception(this IEventWriter writer, EventContext context, EventText eventText, Exception e, string source)
        {
            writer.Exception(context, null, eventText, e, source, null);
        }
        public static void Exception(this IEventWriter writer, EventContext context, EventText eventText, Exception e, IReadOnlyDictionary<string, object> data)
        {
            writer.Exception(context, null, eventText, e, null, data);
        }
        public static void Exception(this IEventWriter writer, EventContext context, EventText eventText, Exception e, string source, IReadOnlyDictionary<string, object> data)
        {
            writer.Exception(context, null, eventText, e, source, data);
        }

        public static void Exception(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText, Exception e)
        {
            writer.Exception(context, category, eventText, e, null, null);
        }
        public static void Exception(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText, Exception e, IReadOnlyDictionary<string, object> data)
        {
            writer.Exception(context, category, eventText, e, null, data);
        }
        public static void Exception(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText, Exception e, string source)
        {
            writer.Exception(context, category, eventText, e, source, null);
        }

        #endregion

        #region Critical

        public static void Critical(this IEventWriter writer, EventContext context, EventText eventText)
        {
            writer.Critical(context, null, eventText, null, null, null);
        }
        public static void Critical(this IEventWriter writer, EventContext context, EventText eventText, string source)
        {
            writer.Critical(context, null, eventText, null, source, null);
        }
        public static void Critical(this IEventWriter writer, EventContext context, EventText eventText, IReadOnlyDictionary<string, object> data)
        {
            writer.Critical(context, null, eventText, null, null, data);
        }
        public static void Critical(this IEventWriter writer, EventContext context, EventText eventText, string source, IReadOnlyDictionary<string, object> data)
        {
            writer.Critical(context, null, eventText, null, source, data);
        }

        public static void Critical(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText)
        {
            writer.Critical(context, category, eventText, null, null, null);
        }
        public static void Critical(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText, string source)
        {
            writer.Critical(context, category, eventText, null, source, null);
        }
        public static void Critical(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText, IReadOnlyDictionary<string, object> data)
        {
            writer.Critical(context, category, eventText, null, null, data);
        }
        public static void Critical(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText, string source, IReadOnlyDictionary<string, object> data)
        {
            writer.Critical(context, category, eventText, null, source, data);
        }


        public static void Critical(this IEventWriter writer, EventContext context, Exception e)
        {
            writer.Critical(context, null, null, e, null, null);
        }
        public static void Critical(this IEventWriter writer, EventContext context, Exception e, string source)
        {
            writer.Critical(context, null, null, e, source, null);
        }
        public static void Critical(this IEventWriter writer, EventContext context, Exception e, IReadOnlyDictionary<string, object> data)
        {
            writer.Critical(context, null, null, e, null, data);
        }
        public static void Critical(this IEventWriter writer, EventContext context, Exception e, string source, IReadOnlyDictionary<string, object> data)
        {
            writer.Critical(context, null, null, e, source, data);
        }

        public static void Critical(this IEventWriter writer, EventContext context, EventCategory category, Exception e)
        {
            writer.Critical(context, category, null, e, null, null);
        }
        public static void Critical(this IEventWriter writer, EventContext context, EventCategory category, Exception e, string source)
        {
            writer.Critical(context, category, null, e, source, null);
        }
        public static void Critical(this IEventWriter writer, EventContext context, EventCategory category, Exception e, IReadOnlyDictionary<string, object> data)
        {
            writer.Critical(context, category, null, e, null, data);
        }
        public static void Critical(this IEventWriter writer, EventContext context, EventCategory category, Exception e, string source, IReadOnlyDictionary<string, object> data)
        {
            writer.Critical(context, category, null, e, source, data);
        }


        public static void Critical(this IEventWriter writer, EventContext context, EventText eventText, Exception e)
        {
            writer.Critical(context, null, eventText, e, null, null);
        }
        public static void Critical(this IEventWriter writer, EventContext context, EventText eventText, Exception e, string source)
        {
            writer.Critical(context, null, eventText, e, source, null);
        }
        public static void Critical(this IEventWriter writer, EventContext context, EventText eventText, Exception e, IReadOnlyDictionary<string, object> data)
        {
            writer.Critical(context, null, eventText, e, null, data);
        }
        public static void Critical(this IEventWriter writer, EventContext context, EventText eventText, Exception e, string source, IReadOnlyDictionary<string, object> data)
        {
            writer.Critical(context, null, eventText, e, source, data);
        }

        public static void Critical(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText, Exception e)
        {
            writer.Critical(context, category, eventText, e, null, null);
        }
        public static void Critical(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText, Exception e, IReadOnlyDictionary<string, object> data)
        {
            writer.Critical(context, category, eventText, e, null, data);
        }
        public static void Critical(this IEventWriter writer, EventContext context, EventCategory category, EventText eventText, Exception e, string source)
        {
            writer.Critical(context, category, eventText, e, source, null);
        }


        #endregion
    }
}
