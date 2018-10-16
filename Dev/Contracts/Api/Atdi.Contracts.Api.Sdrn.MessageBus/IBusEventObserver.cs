using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.Sdrn.MessageBus
{
    public interface IBusEventObserver
    {
        void OnEvent(IBusEvent busEvent);
    }

    public static class BusEventObserverExtention
    {
        private class Event : IBusEvent
        {
            public Event()
            {
                this.Id = Guid.NewGuid();
                this.Created = DateTime.Now;
                this.ManagedThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
            }

            public Guid Id { get; private set; }

            public int Code { get; set; }

            public DateTime Created { get; private set; }

            public BusEventLevel Level { get; set; }

            public string Context { get; set; }

            public string Text { get; set; }

            public int ManagedThread { get; private set; }

            public string Source { get; set; }

            public override string ToString()
            {
                var data = new StringBuilder();

                data.AppendLine($"{this.Created} [#{ManagedThread}] [{this.Level}] ({Code}) {Context} : {Text}");
                return data.ToString();
            }
        }
        public static void Error(this IBusEventObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = BusEventLevel.Error,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Verbouse(this IBusEventObserver observer, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = 0,
                Level = BusEventLevel.Verbouse,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }
        public static void Warning(this IBusEventObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = BusEventLevel.Warning,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Trace(this IBusEventObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = BusEventLevel.Trace,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Debug(this IBusEventObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = BusEventLevel.Debug,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Info(this IBusEventObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = BusEventLevel.Info,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Critical(this IBusEventObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = BusEventLevel.Critical,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Exception(this IBusEventObserver observer, int code, string context, Exception e, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = BusEventLevel.Exception,
                Context = context,
                Source = source?.GetType().Name,
                Text = e.Message
            };

            observer.OnEvent(@event);
        }
    }
}
