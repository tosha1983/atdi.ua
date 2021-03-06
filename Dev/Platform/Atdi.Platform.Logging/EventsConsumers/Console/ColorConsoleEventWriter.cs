﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace Atdi.Platform.Logging.EventsConsumers
{
    public class ColorConsoleEventWriter : IConsoleEventWriter
    {
        private readonly IResourceResolver _resourceResolver;
        private string _offset = "";
        private int _offsetValue;

        public ColorConsoleEventWriter(IResourceResolver resourceResolver)
        {
            this._resourceResolver = resourceResolver;
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

        private void WriteEventRow(IEvent @event)
        {
            //this.Write($"({Thread.CurrentThread.ManagedThreadId}) ", ConsoleColor.Red);

            // the part of time
            var timeFormat = "HH:mm:ss";
            var timeString = @event.Time.ToString(timeFormat);
            this.Write($"{timeString}", ConsoleColor.White);

            var timeFormat2 = "FFF";
            var timeString2 = @event.Time.ToString(timeFormat2).PadRight(timeFormat2.Length, '0');
            this.Write($".{timeString2}", ConsoleColor.Gray);

            var timeFormat3 = "FFFFFFF";
            var timeString3 = @event.Time.ToString(timeFormat3).PadRight(timeFormat3.Length, '0');
            this.Write($"{timeString3.Substring(3)}", ConsoleColor.DarkGray);

            //var ticksString = @event.Time.Ticks.ToString();
            //this.Write($" {ticksString.Substring(ticksString.Length - 12)}", ConsoleColor.Gray);

            // the part of thread
            var treadString = $" #{@event.ManagedThread:D4}";
            this.Write(treadString, ConsoleColor.Yellow);

            // the part of event level
            var levelColor = GetColorbyEventLevel(@event.Level);
            var levelTitle = GetTitleByEventLevel(@event.Level);
            this.Write($" [");
            this.Write($"{levelTitle}", levelColor);
            this.Write($"]");

            // the part of context
            var contextCategorySectionSize = 100;
            var contextName = this._resourceResolver.Resolve(@event.Context.Name);
            if (contextName.Length > contextCategorySectionSize)
            {
                contextName = contextName.Substring(0, contextCategorySectionSize);
                contextCategorySectionSize = 0;
            }
            else
            {
                contextCategorySectionSize -= contextName.Length;
            }
            this.Write(" " + contextName, ConsoleColor.White);

            if (!string.IsNullOrEmpty(@event.Category.Name) && contextCategorySectionSize > 2)
            {
                var category = _resourceResolver.Resolve(@event.Category.Name);
                
                // the part of category
                if (!string.IsNullOrEmpty(category))
                {
                    if (category.Length + 2 > contextCategorySectionSize)
                    {
                        category = category.Substring(0, contextCategorySectionSize - 2);
                        contextCategorySectionSize = 0;
                    }
                    else
                    {
                        contextCategorySectionSize -= category.Length + 2;
                    }
                    this.Write("(", ConsoleColor.White);
                    this.Write(category, ConsoleColor.DarkCyan);
                    this.Write(")", ConsoleColor.White);
                }
            }

            if (contextCategorySectionSize > 0)
            {
                this.Write("".PadRight(contextCategorySectionSize, ' '), ConsoleColor.White);
            }

            string eventText = null;
            var text = @event.Text;
            if (!string.IsNullOrEmpty(text.Text))
            {
                eventText = this._resourceResolver.Resolve(text.Text, text.Args);
            }
            else if (@event is IDebugEvent && !(@event is IBeginTraceEvent) && !(@event is IEndTraceEvent))
            {
                eventText = "-- Data info --";
            }
            this.Write(Environment.NewLine + "     ");
            // the part of trace
            if (@event is ITraceEvent traceEvent)
            {
                if (traceEvent.Duration.HasValue)
                {
                    var durationMsString = traceEvent.Duration.Value.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
                    //var durationTksString = traceEvent.Duration.Value.Ticks.ToString();

                    this.Write($"+", ConsoleColor.White);
                    this.Write($"{durationMsString}", levelColor);
                    this.Write($"ms ", ConsoleColor.White);
                }

                if (traceEvent is IBeginTraceEvent beginTraceEvent)
                {
                    this.Write($"Begin scope '", ConsoleColor.White);
                    this.Write($"{beginTraceEvent.ScopeData.Name}", levelColor);
                    this.Write($"'", ConsoleColor.White);
                }
                else
                {
                    if (traceEvent is IEndTraceEvent endTraceEvent)
                    {
                        this.Write($"End scope '", ConsoleColor.White);
                        this.Write($"{endTraceEvent.ScopeData.Name}", levelColor);
                        this.Write($"'", ConsoleColor.White);
                    }
                    else
                    {
                        this.Write($"Scope '", ConsoleColor.White);
                        this.Write($"{traceEvent.ScopeData.Name}", levelColor);
                        this.Write($"'", ConsoleColor.White);
                    }
                }
                

                if (!string.IsNullOrEmpty(eventText))
                {
                    this.Write($" >> '", ConsoleColor.White);
                    this.Write($"{eventText}", levelColor);
                    this.Write($"'", ConsoleColor.White);
                }
                // the part of event text
                // the part of duration
            }
            else
            {
                if (@event is IExceptionEvent exceptionEvent)
                {
                    this.Write($": '", ConsoleColor.White);
                    this.Write($"{exceptionEvent.Exception.Message.Replace(Environment.NewLine, " ")}", levelColor);
                    this.Write($"'", ConsoleColor.White);

                    if (!string.IsNullOrEmpty(eventText))
                    {
                        this.Write($" >> '", ConsoleColor.White);
                        this.Write($"{eventText}", levelColor);
                        this.Write($"'", ConsoleColor.White);
                    }
                }
                else if(@event is ICriticalEvent criticalEvent && criticalEvent.Exception != null)
                {
                    this.Write($": '", ConsoleColor.White);
                    this.Write($"{criticalEvent.Exception.Message.Replace(Environment.NewLine, " ")}", levelColor);
                    this.Write($"'", ConsoleColor.White);

                    if (!string.IsNullOrEmpty(eventText))
                    {
                        this.Write($" >> '", ConsoleColor.White);
                        this.Write($"{eventText}", levelColor);
                        this.Write($"'", ConsoleColor.White);
                    }
                }
                else
                {
                    // the user event text
                    this.Write($"'", ConsoleColor.White);
                    //this.Write($"{eventText.Replace(":", ":" + Environment.NewLine + "                                                                     ")}", levelColor);
                    //this.Write($"{Wrap(eventText, 100, Environment.NewLine + "                                                                    ")}");
                    this.Write($"{Wrap(eventText, 100, Environment.NewLine + "      ")}",levelColor);
                    this.Write($"'", ConsoleColor.White);
                }
            }

            if (@event is IDebugEvent debugEvent)
            {
                if (!string.IsNullOrEmpty(debugEvent.Source))
                {
                    this.Write($" [", ConsoleColor.White);
                    this.Write($"{debugEvent.Source}", levelColor);
                    this.Write($"]", ConsoleColor.White);
                }
            }

            this.Write(Environment.NewLine);
        }

        public static string Wrap(string text, int margin, string separator)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            int start = 0, end;
            var lines = new List<string>(text.Length / margin + 1);
            //text = Regex.Replace(text, @"\s", " ").Trim();

            while ((end = start + margin) < text.Length)
            {
                while (text[end] != ' ' && end > start)
                    end -= 1;

                if (end == start)
                    end = start + margin;

                lines.Add(text.Substring(start, end - start));
                start = end + 1;
            }

            if (start < text.Length)
                lines.Add(text.Substring(start));

            return string.Join(separator, lines.ToArray());
        }

        private ConsoleColor GetColorbyEventLevel(EventLevel level)
        {
            switch (level)
            {
                case EventLevel.Trace:
                    return ConsoleColor.DarkGreen;
                case EventLevel.Debug:
                    return ConsoleColor.Green;
                case EventLevel.Verbouse:
                    return ConsoleColor.Gray;
                case EventLevel.Info:
                    return ConsoleColor.Cyan;
                case EventLevel.Warning:
                    return ConsoleColor.Yellow;
                case EventLevel.Error:
                    return ConsoleColor.Magenta ;
                case EventLevel.Exception:
                    return ConsoleColor.Red;
                case EventLevel.Critical:
                    return ConsoleColor.Red;
                default:
                    return ConsoleColor.White;
            }
        }

        private string GetTitleByEventLevel(EventLevel level)
        {
            switch (level)
            {
                case EventLevel.Trace:
                    return "--Trace--";
                case EventLevel.Debug:
                    return "--Debug--";
                case EventLevel.Verbouse:
                    return "-Verbose-";
                case EventLevel.Info:
                    return "--Info---";
                case EventLevel.Warning:
                    return "--Warn---";
                case EventLevel.Error:
                    return "--Error--";
                case EventLevel.Exception:
                    return "Exception";
                case EventLevel.Critical:
                    return "Critical-";
                default:
                    return "Unknown";
            }
        }

        private void WriteEventDebugInfo(IDebugEvent @event)
        {
            if (@event.Data == null)
                return;

            //this.WriteLine("");
            //if (string.IsNullOrEmpty(@event.Text.Text))
            //{
            //    this.WriteLine(" -- Data info -- ");
            //}
            
            //this.WriteLine("");

            foreach (var item in @event.Data)
            {
                this.WriteBeginLine($" {item.Key}", ConsoleColor.DarkYellow);
                this.Write($" = '", ConsoleColor.White);
                this.Write($"{item.Value}", ConsoleColor.Gray);
                this.WriteEndLine($"'", ConsoleColor.White);
            }
        }

        private void WriteEventExceptionInfo(IExceptionData exception, bool isInner = false)
        {
            var propColor = ConsoleColor.DarkYellow;
            var valueColor = ConsoleColor.DarkCyan;

            if (!isInner)
            {
                //this.WriteLine("");
                this.WriteLine(" -- Exception detail -- ", ConsoleColor.White);
                //this.WriteLine("");
            }
            else
            {
                //this.WriteLine("");
                this.WriteLine(" -- Inner exception detail -- ", ConsoleColor.White);
                //this.WriteLine("");
            }
            

            this.WriteBeginLine($" Message", propColor);
            this.Write($": '", ConsoleColor.White);
            this.Write(exception.Message.Replace(Environment.NewLine, " "), valueColor);
            this.WriteEndLine($"'", ConsoleColor.White);

            this.WriteBeginLine($"    Type", propColor);
            this.Write($": '", ConsoleColor.White);
            this.Write(exception.Type, valueColor);
            this.WriteEndLine($"'", ConsoleColor.White);

            this.WriteBeginLine($"  Source", propColor);
            this.Write($": '", ConsoleColor.White);
            this.Write(exception.Source, valueColor);
            this.WriteEndLine($"'", ConsoleColor.White);

            this.WriteBeginLine($"  Target", propColor);
            this.Write($": '", ConsoleColor.White);
            this.Write(exception.TargetSite, valueColor);
            this.WriteEndLine($"'", ConsoleColor.White);

            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                this.WriteLine($" -- Stack begin --", propColor);
                this.WriteLine(exception.StackTrace.Replace(Environment.NewLine, Environment.NewLine + _offset));
                this.WriteLine(" -- Stack end --", propColor);
            }

            if (exception.Inner != null)
            {
                this.PushOffset();
                this.WriteEventExceptionInfo(exception.Inner, true);
                this.PopOffset();
            }
            else
            {
                //this.WriteLine("");
            }
        }

        public void Write(IEvent @event)
        {
            try
            {
                this.WriteEventRow(@event);

                if (@event.Level == EventLevel.Verbouse ||
                    @event.Level == EventLevel.Info ||
                    @event.Level == EventLevel.Warning)
                {
                    return;
                }

                this.PushOffset();
                if (@event is IDebugEvent)
                {

                    this.WriteEventDebugInfo((IDebugEvent)@event);
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
                    this.WriteEventExceptionInfo(exception);
                }

                this.PopOffset();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e.ToString());
                Console.WriteLine();
            }
            
        }


        private void SetColor(ConsoleColor? background, ConsoleColor? foreground)
        {
            if (background.HasValue)
            {
                Console.BackgroundColor = background.Value;
            }
            if (foreground.HasValue)
            {
                Console.ForegroundColor = foreground.Value;
            }
        }

        public void WriteBeginLine(string message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            this.SetColor(background, foreground);
            Console.Out.Write(_offset + message);
            Console.ResetColor();
        }

        public void Write(string message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            this.SetColor(background, foreground);
            Console.Out.Write(message);
            Console.ResetColor();
        }

        public void WriteEndLine(string message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            this.SetColor(background, foreground);
            Console.Out.Write(message);
            Console.ResetColor();
            Console.Out.Write(Environment.NewLine);
        }

        public void WriteLine(string message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            this.SetColor(background, foreground);
            Console.Out.WriteLine(_offset + message);
            Console.ResetColor();
        }
    }
}
