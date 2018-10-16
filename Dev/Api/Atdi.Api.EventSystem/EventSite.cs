using Atdi.Contracts.Api.EventSystem;
using Atdi.Modules.AmqpBroker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EventSystem
{
    public class EventSite : IEventSite
    {
        private readonly IEventSiteConfig _config;
        private readonly ISubscriberActivator _activator;
        private readonly IEventSystemObserver _observer;
        private readonly ConnectionFactory _connectionFactory;

        public EventSite(IEventSiteConfig config, ISubscriberActivator activator, IEventSystemObserver observer)
        {
            this._config = config;
            this._activator = activator;
            this._observer = observer;
            this._connectionFactory = new ConnectionFactory(new AmqpBrokerObserver(observer));
            this._observer.Verbouse("EventSystem.Initialize", $"The event system is initialized successfully", this);
        }

        public void Dispose()
        {
        
        }

        internal ISubscriberActivator Activator { get => _activator; }

        public IEventDispatcher GetDispatcher()
        {
            return new EventDispatcher(this);
        }

        public IEventEmitter GetEmitter()
        {
            return new EventEmitter(this);
        }
    }
}
