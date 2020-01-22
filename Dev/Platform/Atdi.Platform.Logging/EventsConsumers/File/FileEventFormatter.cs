using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Atdi.Platform.Logging.EventsConsumers
{
    public sealed class FileEventFormatter : IEventFormatter
    {
        private readonly FileEventsConsumerConfig _config;
        private readonly IResourceResolver _resourceResolver;
        private string _offset = "";
        private int _offsetValue = 0;

        private readonly StringBuilder _buffer;

        public FileEventFormatter(FileEventsConsumerConfig config, IResourceResolver resourceResolver)
        {
            this._resourceResolver = resourceResolver;
            this._config = config;
			this._buffer = new StringBuilder();
        }

        private void RecalcOffset()
        {
            _offset = new string(' ', _offsetValue * 4);
        }

        private void PushOffset()
        {
            ++_offsetValue;
            RecalcOffset();
        }

        private void PopOffset()
        {
            --_offsetValue;
            RecalcOffset();
        }

        private void WriteEventRow(IEvent @event, StringBuilder buffer)
        {
            // the part of time
            const string timeFormat = "HH:mm:ss";
            var timeString = @event.Time.ToString(timeFormat);
            buffer.Append(timeString);

            const string timeFormat2 = "FFF";
            var timeString2 = @event.Time.ToString(timeFormat2).PadRight(timeFormat2.Length, '0');
            buffer.Append($".", timeString2);

            const string timeFormat3 = "FFFFFFF";
            var timeString3 = @event.Time.ToString(timeFormat3).PadRight(timeFormat3.Length, '0');
            buffer.Append($".", timeString3.Substring(3));

            // the part of thread
            buffer.Append($" ", @event.ManagedThread.ToString());

            // the part of event level
            var levelTitle = GetTitleByEventLevel(@event.Level);
            buffer.Append($" ", levelTitle);

            // the part of context
            buffer.Append(" ", this._resourceResolver.Resolve(@event.Context.Name));

            string eventText = null;
            var text = @event.Text;
            if (!string.IsNullOrEmpty(text.Text))
            {
                eventText = this._resourceResolver.Resolve(text.Text, text.Args);
            }

            if (!string.IsNullOrEmpty(@event.Category.Name))
            {
                var category = _resourceResolver.Resolve(@event.Category.Name);
                // the part of category
                if (!string.IsNullOrEmpty(category))
                {
                    buffer.Append($"(", category, ")");
                }
            }

            // the part of trace
            if (@event is ITraceEvent traceEvent)
            {
                if (traceEvent is IBeginTraceEvent beginTraceEvent)
                {
                    var scopeName = beginTraceEvent.ScopeData.Name?.ToString();
                    if (string.IsNullOrEmpty(scopeName))
                    {
                        buffer.Append($": Begin");
                    }
                    else
                    {
                        buffer.Append($": Begin '", scopeName, "'");
                    }
                        
                }
                else
                {
                    if (traceEvent is IEndTraceEvent endTraceEvent)
                    {
                        var scopeName = endTraceEvent.ScopeData.Name?.ToString();
                        if (string.IsNullOrEmpty(scopeName))
                        {
                            buffer.Append($": End");
                        }
                        else
                        {
                            buffer.Append($": End '", scopeName, "'");
                        }
                        
                    }
                    else
                    {
                        buffer.Append($": Scope '", traceEvent.ScopeData.Name?.ToString(), "'");
                    }
                }
                if (traceEvent.Duration.HasValue)
                {
                    var durationMsString = traceEvent.Duration.Value.TotalMilliseconds.ToString();
                    //var durationTksString = traceEvent.Duration.Value.Ticks.ToString();

                    buffer.Append($" (+", durationMsString, "ms)");
                }

                if (!string.IsNullOrEmpty(eventText))
                {
                    buffer.Append($" '", eventText, "'");
                }
            }
            else
            {
                if (@event is IExceptionEvent exceptionEvent)
                {
                    buffer.Append($": '", exceptionEvent.Exception.Message.Replace(Environment.NewLine, " "), "'");
                    if (!string.IsNullOrEmpty(eventText))
                    {
                        buffer.Append($" >> '", eventText, "'");
                    }
                }
                else if (@event is ICriticalEvent criticalEvent && criticalEvent.Exception != null)
                {
                    buffer.Append($": '", criticalEvent.Exception.Message.Replace(Environment.NewLine, " "), "'");

                    if (!string.IsNullOrEmpty(eventText))
                    {
                        buffer.Append($" >> '", eventText, "'");
                    }
                }
                else
                {
                    // the user event text
                    buffer.Append($": '", eventText, "'");
                }
            }

            if (@event is IDebugEvent debugEvent)
            {
                if (!string.IsNullOrEmpty(debugEvent.Source))
                {
                    buffer.Append($" [", debugEvent.Source, "]");
                }
            }

            
        }

        

        private static string GetTitleByEventLevel(EventLevel level)
        {
            switch (level)
            {
                case EventLevel.Trace:
                    return "Trc";
                case EventLevel.Debug:
                    return "Dbg";
                case EventLevel.Verbouse:
                    return "Vrb";
                case EventLevel.Info:
                    return "Inf";
                case EventLevel.Warning:
                    return "Wrn";
                case EventLevel.Error:
                    return "Err";
                case EventLevel.Exception:
                    return "Exp";
                case EventLevel.Critical:
                    return "Crl";
                default:
                    return "Unknown";
            }
        }

        private void WriteEventDebugInfo(IDebugEvent @event, StringBuilder buffer)
        {
            if (@event.Data == null)
                return;

            buffer.Append(Environment.NewLine);
            buffer.AppendLine(_offset, " -- Data info -- ");

            foreach (var item in @event.Data)
            {
                buffer.AppendLine(_offset, $" ", item.Key, " = '", item.Value, "'");
            }
        }

        private void WriteEventExceptionInfo(IExceptionData exception, StringBuilder buffer, bool isInner = false)
        {
            if (!isInner)
            {
                buffer.Append(Environment.NewLine);
                buffer.AppendLine(_offset, " -- Exception detail -- ");
            }
            else
            {
                buffer.AppendLine(_offset, " -- Inner exception detail -- ");
            }

            buffer.AppendLine(_offset, $" Message: '", exception.Message.Replace(Environment.NewLine, " "), "'");
            buffer.AppendLine(_offset, $"    Type: '", exception.Type, "'");
            buffer.AppendLine(_offset, $"  Source: '",exception.Source,"'");
            buffer.AppendLine(_offset, $"  Target: '", exception.TargetSite, "'");

            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                buffer.AppendLine(_offset, $" -- Stack begin --");
                buffer.AppendLine(_offset, exception.StackTrace.Replace(Environment.NewLine, Environment.NewLine + _offset));
                buffer.AppendLine(_offset, " -- Stack end --");
            }

            if (exception.Inner != null)
            {
                this.PushOffset();
                this.WriteEventExceptionInfo(exception.Inner, buffer, true);
                this.PopOffset();
            }
            
        }

        public string Format(IEvent @event)
        {
            //var buffer = new StringBuilder();
            _buffer.Clear();

            this.WriteEventRow(@event, _buffer);

            if (@event.Level == EventLevel.Verbouse ||
                @event.Level == EventLevel.Info ||
                @event.Level == EventLevel.Warning)
            {
                return _buffer.ToString();
            }

            this.PushOffset();

            if (@event is IDebugEvent)
            {

                this.WriteEventDebugInfo((IDebugEvent)@event, _buffer);
            }

            IExceptionData exception = null;
            if (@event.Level == EventLevel.Exception)
            {
                exception = ((IExceptionEvent)@event).Exception;
            }
            if (@event.Level == EventLevel.Critical)
            {
                exception = ((ICriticalEvent)@event).Exception;
            }

            if (exception != null)
            {
                this.WriteEventExceptionInfo(exception, _buffer);
            }

            this.PopOffset();

            return _buffer.ToString();
        }
    }

    public static class StringBuilderExtension
    {
	    public static void AppendLine(this StringBuilder builder, string value1, string value2)
	    {
		    builder.Append(value1);
		    builder.Append(value2);
		    builder.Append(Environment.NewLine);
	    }
	    public static void AppendLine(this StringBuilder builder, string value1, string value2, string value3)
	    {
		    builder.Append(value1);
		    builder.Append(value2);
		    builder.Append(value3);
			builder.Append(Environment.NewLine);
	    }

	    public static void AppendLine(this StringBuilder builder, string value1, string value2, string value3, string value4)
	    {
		    builder.Append(value1);
		    builder.Append(value2);
		    builder.Append(value3);
		    builder.Append(value4);
			builder.Append(Environment.NewLine);
	    }
	    public static void AppendLine(this StringBuilder builder, string value1, string value2, string value3, string value4, string value5)
	    {
		    builder.Append(value1);
		    builder.Append(value2);
		    builder.Append(value3);
		    builder.Append(value4);
		    builder.Append(value5);
			builder.Append(Environment.NewLine);
	    }
	    public static void AppendLine(this StringBuilder builder, string value1, string value2, string value3, string value4, string value5, string value6)
	    {
		    builder.Append(value1);
		    builder.Append(value2);
		    builder.Append(value3);
		    builder.Append(value4);
		    builder.Append(value5);
		    builder.Append(value6);
			builder.Append(Environment.NewLine);
	    }

		public static void AppendLine(this StringBuilder builder, params string[] values)
	    {
		    foreach (var value in values)
		    {
			    builder.Append(value);
		    }	

		    builder.Append(Environment.NewLine);
	    }

	    public static void Append(this StringBuilder builder, string value1, string value2)
	    {
			builder.Append(value1);
			builder.Append(value2);
		}
	    public static void Append(this StringBuilder builder, string value1, string value2, string value3)
	    {
		    builder.Append(value1);
		    builder.Append(value2);
		    builder.Append(value3);
		}
	    public static void Append(this StringBuilder builder, string value1, string value2, string value3, string value4)
	    {
		    builder.Append(value1);
		    builder.Append(value2);
		    builder.Append(value3);
		    builder.Append(value4);
	    }

		public static void Append(this StringBuilder builder, params string[] values)
	    {
		    foreach (var value in values)
		    {
			    builder.Append(value);
		    }
	    }
	}
}
