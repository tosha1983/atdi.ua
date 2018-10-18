using Atdi.Contracts.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EventSystem
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly EventSite _eventSite;
        private readonly IEventSystemObserver _observer;
        private readonly EventSystemConfig _sysConfig;
        private readonly Dictionary<string, EventDeliveryHandler> _deliveryHandlers;

        public EventDispatcher(IEventSite eventSite, IEventSystemObserver observer)
        {
            this._eventSite = eventSite as EventSite;
            this._sysConfig = this._eventSite.SysConfig;
            this._deliveryHandlers = new Dictionary<string, EventDeliveryHandler>();
            _observer = observer;
        }

        public void Dispose()
        {
            if (_deliveryHandlers != null && _deliveryHandlers.Count > 0)
            {
                var handlers = _deliveryHandlers.Values.ToArray();
                for (int i = 0; i < handlers.Length; i++)
                {
                    try
                    {
                        handlers[i].Dispose();
                    }
                    catch (Exception e)
                    {
                        _observer.Exception(EventSystemEvents.EventDispatcherDisposeExeption, "EventSystem.Dispose", e, this);
                    }

                }
                _deliveryHandlers.Clear();
            }
        }

        public void Subscribe(string eventName, Type type, string subscriberName = null)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                throw new ArgumentException(nameof(eventName));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(subscriberName))
            {
                subscriberName = type.FullName;
            }

            var key = $"{eventName}.{subscriberName}";

            if (this._deliveryHandlers.ContainsKey(key))
            {
                throw new InvalidOperationException($"The subscriber '{subscriberName}' is already subscribed to the event '{eventName}'.");
            }

            using (var declareChannel = this._eventSite.DeclaratorConnection.CreateChannel())
            {
                var exchange = this._sysConfig.BuildEventExchangeName();
                declareChannel.DeclareDurableQueue(this._sysConfig.BuildCommonLogQueueName(), exchange, $"[@{eventName}]");
                declareChannel.DeclareDurableQueue(this._sysConfig.BuildEventLogQueueName(eventName), exchange, $"[@{eventName}]");
                declareChannel.DeclareDurableQueue(this._sysConfig.BuildEventQueueName(eventName, subscriberName), exchange, $"[@{eventName}]");
                declareChannel.DeclareDurableQueue(this._sysConfig.BuildEventErrorsQueueName(eventName, subscriberName), exchange, $"[@{eventName}].[error]");
            }

            var deliveryHandler = new EventDeliveryHandler(eventName, type, subscriberName, _eventSite, _observer);
            _deliveryHandlers.Add(key, deliveryHandler);
            deliveryHandler.Join();
        }

        public void Unsubscribe(string eventName, Type type, string subscriberName = null)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                throw new ArgumentException(nameof(eventName));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(subscriberName))
            {
                subscriberName = type.FullName;
            }

            var key = $"{eventName}.{subscriberName}";

            if (!this._deliveryHandlers.ContainsKey(key))
            {
                return;
            }

            var deliveryHandler = _deliveryHandlers[key];
            deliveryHandler.Dispose();
            _deliveryHandlers.Remove(key);
        }
    }
}
