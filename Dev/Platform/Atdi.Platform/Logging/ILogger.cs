using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    public interface ILogger : IEventWriter, IDisposable
    {
        bool IsAllowed(EventLevel level);
    }

    public static class LoggerExtension
    {
        private static string BuildSource(object source, string memberName)
        {
            if (source == null)
            {
                return memberName;
            }

            return $"{source.GetType().Name}.{memberName}";
        }
        public static void Exception(this ILogger writer, EventContext context, EventCategory category, EventText eventText, Exception e, object source, [CallerMemberName] string memberName = null)
        {
            writer.Exception(context, category, eventText, e, BuildSource(source, memberName), null);
        }

        public static void Exception(this ILogger writer, EventContext context, EventCategory category, Exception e, object source, [CallerMemberName] string memberName = null)
        {
            writer.Exception(context, category, e, BuildSource(source, memberName), (IReadOnlyDictionary<string, object>)null);
        }

        public static void Critical(this ILogger writer, EventContext context, EventCategory category, Exception e, object source, [CallerMemberName] string memberName = null)
        {
	        writer.Critical(context, category, e, BuildSource(source, memberName), (IReadOnlyDictionary<string, object>)null);
        }

		public static ITraceScope StartTrace(this ILogger writer, EventContext context, EventCategory category, object source, [CallerMemberName] string memberName = null)
        {
            return writer.StartTrace(context, category, BuildSource(source, memberName));
        }
    }
}
