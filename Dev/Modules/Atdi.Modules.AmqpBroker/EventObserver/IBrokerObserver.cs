using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.AmqpBroker
{
    public interface IBrokerObserver
    {
        void OnEvent(IBrokerEvent brokerEvent);
    }

    public static class BrokerObserverExtention
    {
        private class Event : IBrokerEvent
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

            public BrokerEventLevel Level { get; set; }

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
        public static void Error(this IBrokerObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = BrokerEventLevel.Error,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Verbouse(this IBrokerObserver observer, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = 0,
                Level = BrokerEventLevel.Verbouse,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }
        public static void Warning(this IBrokerObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = BrokerEventLevel.Warning,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Trace(this IBrokerObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = BrokerEventLevel.Trace,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Debug(this IBrokerObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = BrokerEventLevel.Debug,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Info(this IBrokerObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = BrokerEventLevel.Info,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Critical(this IBrokerObserver observer, int code, string context, string text, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = BrokerEventLevel.Critical,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            observer.OnEvent(@event);
        }

        public static void Exception(this IBrokerObserver observer, int code, string context, Exception e, object source)
        {
            if (observer == null)
            {
                return;
            }

            var @event = new Event
            {
                Code = code,
                Level = BrokerEventLevel.Exception,
                Context = context,
                Source = source?.GetType().Name,
                Text = e.Message,
                Exception = e
            };

            observer.OnEvent(@event);
        }
    }
}
