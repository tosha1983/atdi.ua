using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal class BusLogger
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
                Code = BusEvents.VebouseEvent,
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

        public void Exception(int code, string context, Exception e, object source)
        {
            if (_observer == null)
            {
                return;
            }

            var @event = new BusEvent
            {
                Code = code,
                Level = BusEventLevel.Exception,
                Context = context,
                Source = source?.GetType().Name,
                Text = e.Message
            };

            _observer.OnEvent(@event);
        }
    }
}
