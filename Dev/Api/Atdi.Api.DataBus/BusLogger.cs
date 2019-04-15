using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using Atdi.Modules.AmqpBroker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.DataBus
{
    internal sealed class BusLogger: IBrokerObserver
    {
        private readonly IBusEventObserver _observer;

        public BusLogger(IBusEventObserver observer)
        {
            this._observer = observer;
        }

        public void Error(int code, string context, string text, object source)
        {
            if (_observer == null)
            {
                return;
            }

            var @event = new BusEvent
            {
                Code = code,
                Level = BusEventLevel.Error,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            _observer.OnEvent(@event);
        }

        public void Verbouse(string context, string text, object source)
        {
            if (_observer == null)
            {
                return;
            }

            var @event = new BusEvent
            {
                Code = BusEvents.VerbouseEvent,
                Level = BusEventLevel.Verbouse,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            _observer.OnEvent(@event);
        }

        public void Warning(int code, string context, string text, object source)
        {
            if (_observer == null)
            {
                return;
            }

            var @event = new BusEvent
            {
                Code = code,
                Level = BusEventLevel.Warning,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            _observer.OnEvent(@event);
        }

        public void Trace(int code, string context, string text, object source)
        {
            if (_observer == null)
            {
                return;
            }

            var @event = new BusEvent
            {
                Code = code,
                Level = BusEventLevel.Trace,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            _observer.OnEvent(@event);
        }

        public void Debug(int code, string context, string text, object source)
        {
            if (_observer == null)
            {
                return;
            }

            var @event = new BusEvent
            {
                Code = code,
                Level = BusEventLevel.Debug,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            _observer.OnEvent(@event);
        }

        public void Info(int code, string context, string text, object source)
        {
            if (_observer == null)
            {
                return;
            }

            var @event = new BusEvent
            {
                Code = code,
                Level = BusEventLevel.Info,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            _observer.OnEvent(@event);
        }

        public void Critical(int code, string context, string text, object source)
        {
            if (_observer == null)
            {
                return;
            }

            var @event = new BusEvent
            {
                Code = code,
                Level = BusEventLevel.Critical,
                Context = context,
                Source = source?.GetType().Name,
                Text = text
            };

            _observer.OnEvent(@event);
        }

        public void Exception(string context, Exception e, object source)
        {
            if (_observer == null)
            {
                return;
            }

            var @event = new BusEvent
            {
                Code = BusEvents.ExceptionEvent,
                Level = BusEventLevel.Exception,
                Context = context,
                Source = source?.GetType().Name,
                Text = e.Message,
                Exception = e
            };

            _observer.OnEvent(@event);
        }

        public void Exception(string context, string text, Exception e, object source)
        {
            if (_observer == null)
            {
                return;
            }

            var @event1 = new BusEvent
            {
                Code = BusEvents.ExceptionEvent,
                Level = BusEventLevel.Exception,
                Context = context,
                Source = source?.GetType().Name,
                Text = e.Message,
                Exception = e
            };

            _observer.OnEvent(@event1);

            var @event2 = new BusEvent
            {
                Code = BusEvents.ExceptionEvent,
                Level = BusEventLevel.Error,
                Context = context,
                Source = source?.GetType().Name,
                Text = text 
            };

            _observer.OnEvent(@event2);
        }

        void IBrokerObserver.OnEvent(IBrokerEvent brokerEvent)
        {
            if (_observer == null)
            {
                return;
            }

            var @event = new BusEvent(brokerEvent.Id, brokerEvent.Created, brokerEvent.ManagedThread)
            {
                Code = brokerEvent.Code ,
                Context = brokerEvent.Context,
                Source = brokerEvent.Source,
                Text = brokerEvent.Text
            };

            switch (brokerEvent.Level)
            {
                case BrokerEventLevel.None:
                    @event.Level = BusEventLevel.None;
                    break;
                case BrokerEventLevel.Verbouse:
                    @event.Level = BusEventLevel.Verbouse;
                    break;
                case BrokerEventLevel.Info:
                    @event.Level = BusEventLevel.Info;
                    break;
                case BrokerEventLevel.Warning:
                    @event.Level = BusEventLevel.Warning;
                    break;
                case BrokerEventLevel.Trace:
                    @event.Level = BusEventLevel.Trace;
                    break;
                case BrokerEventLevel.Debug:
                    @event.Level = BusEventLevel.Debug;
                    break;
                case BrokerEventLevel.Error:
                    @event.Level = BusEventLevel.Error;
                    break;
                case BrokerEventLevel.Exception:
                    @event.Level = BusEventLevel.Exception;
                    break;
                case BrokerEventLevel.Critical:
                    @event.Level = BusEventLevel.Critical;
                    break;
                default:
                    @event.Level = BusEventLevel.None;
                    break;
            }
            _observer.OnEvent(@event);
        }
    }
}
