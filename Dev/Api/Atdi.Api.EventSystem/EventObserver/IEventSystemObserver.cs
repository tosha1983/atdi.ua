using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EventSystem
{
    public interface IEventSystemObserver
    {
        void OnEvent(IEventSystemEvent @event);
    }

    public static class EventSystemObserverExtention
    {
        private class Event : IEventSystemEvent
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

            public EventSystemEventLevel Level { get; set; }

            public string Context { get; set; }

            public string Text { get; set; }

            public int ManagedThread { get; private set; }

            public string Source { get; set; }

            public Exception Exception { get; set; }

            public override string ToString()
            {
                var data = new StringBuilder();

                data.AppendLine($"{this.Created} [#{ManagedThread}] [{this.Level}] ({Code}) {Context} : {Text}");
                return data.ToString();
            }
        }
        public static void Error(this IEventSystemObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = EventSystemEventLevel.Error,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Verbouse(this IEventSystemObserver observer, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = 0,
                Level = EventSystemEventLevel.Verbouse,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }
        public static void Warning(this IEventSystemObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = EventSystemEventLevel.Warning,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Trace(this IEventSystemObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = EventSystemEventLevel.Trace,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Debug(this IEventSystemObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = EventSystemEventLevel.Debug,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Info(this IEventSystemObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = EventSystemEventLevel.Info,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Critical(this IEventSystemObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = EventSystemEventLevel.Critical,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Exception(this IEventSystemObserver observer, int code, string context, Exception e, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = EventSystemEventLevel.Exception,
                Context = context,
                Source = source?.GetType().Name,
                Text = e.Message,
                Exception = e
            };

            observer.OnEvent(@event);
        }
    }
}
