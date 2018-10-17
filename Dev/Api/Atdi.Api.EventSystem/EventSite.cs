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
        private readonly object _emitterConnectionLocker = new object();
        private readonly object _dispatcherConnectionLocker = new object();
        private readonly object _declaratorConnectionLocker = new object();

        private readonly IEventSiteConfig _config;
        private readonly ISubscriberActivator _activator;
        private readonly IEventSystemObserver _observer;
        private readonly ConnectionFactory _connectionFactory;
        private Connection _emitterConnection;
        private Connection _dispatcherConnection;
        private Connection _declaratorConnection;

        private readonly string _commonLogQueue;
        public EventSite(IEventSiteConfig config, ISubscriberActivator activator, IEventSystemObserver observer)
        {
            this._config = config;
            this._activator = activator;
            this._observer = observer;
            this._connectionFactory = new ConnectionFactory(new AmqpBrokerObserver(observer));
            this._observer.Verbouse("EventSystem.Initialize", $"The event system is initialized successfully", this);
            this._commonLogQueue = $"{config.GetValue<string>(EventSiteConfig.EventQueueNamePart)}.[Events].[log]";

            using (var declaratorChannel = this.DeclaratorConnection.CreateChannel())
            {
                declaratorChannel.DeclareDurableDirectExchange(config.GetValue<string>(EventSiteConfig.EventExchange));
                declaratorChannel.DeclareDurableQueue(this._commonLogQueue);
            }
        }

        public void Dispose()
        {
            if (_emitterConnection != null)
            {
                lock (_emitterConnectionLocker)
                {
                    if (_emitterConnection != null)
                    {
                        try
                        {
                            _emitterConnection.Dispose();
                        }
                        catch(Exception e)
                        {
                            _observer.Exception(EventSystemEvents.EventSiteDisposeExeption, "EventSystem.Dispose", e, this);
                        }
                        _emitterConnection = null;
                    }
                }
            }

            if (_dispatcherConnection != null)
            {
                lock (_dispatcherConnectionLocker)
                {
                    if (_dispatcherConnection != null)
                    {
                        try
                        {
                            _dispatcherConnection.Dispose();
                        }
                        catch (Exception e)
                        {
                            _observer.Exception(EventSystemEvents.EventSiteDisposeExeption, "EventSystem.Dispose", e, this);
                        }
                        _dispatcherConnection = null;
                    }
                }
            }


            if (_declaratorConnection != null)
            {
                lock (_declaratorConnectionLocker)
                {
                    if (_declaratorConnection != null)
                    {
                        try
                        {
                            _declaratorConnection.Dispose();
                        }
                        catch (Exception e)
                        {
                            _observer.Exception(EventSystemEvents.EventSiteDisposeExeption, "EventSystem.Dispose", e, this);
                        }
                        _declaratorConnection = null;
                    }
                }
            }
        }

        internal string CommonLogQueueName { get => this._commonLogQueue; }

        internal ISubscriberActivator Activator { get => _activator; }

        internal Connection EmitterConnection
        {
            get
            {
                if (_emitterConnection == null)
                {
                    lock(_emitterConnectionLocker)
                    {
                        if (_emitterConnection == null)
                        {
                            var c = CreateConnection("EventEmmitter");
                            _emitterConnection = c;
                        }
                    }
                }
                return _emitterConnection;
            }
        }

        internal Connection DispatcherConnection
        {
            get
            {
                if (_dispatcherConnection == null)
                {
                    lock (_dispatcherConnectionLocker)
                    {
                        if (_dispatcherConnection == null)
                        {
                            var c = CreateConnection("EventDispatcher");
                            _dispatcherConnection = c;
                        }
                    }
                }
                return _dispatcherConnection;
            }
        }

        internal Connection DeclaratorConnection
        {
            get
            {
                if (_declaratorConnection == null)
                {
                    lock (_declaratorConnectionLocker)
                    {
                        if (_declaratorConnection == null)
                        {
                            var c = CreateConnection("EventDeclarator");
                            _declaratorConnection = c;
                        }
                    }
                }
                return _declaratorConnection;
            }
        }

        public IEventSiteConfig Config => _config;

        private Connection CreateConnection(string name)
        {
            var config = new ConnectionConfig
            {
                AutoRecovery = true,
                ConnectionName = $"EventSystem.[{_config.GetValue<string>(EventSiteConfig.AppName)}].[{name}].[v{_config.GetValue<string>(EventSiteConfig.ApiVersion)}]",
                HostName = _config.GetValue<string>(EventSiteConfig.EventBusHost),
                VirtualHost = _config.GetValue<string>(EventSiteConfig.EventBusVirtualHost),
                Port = _config.GetValue<int?>(EventSiteConfig.EventBusPort),
                UserName = _config.GetValue<string>(EventSiteConfig.EventBusUser),
                Password = _config.GetValue<string>(EventSiteConfig.EventBusPassword)
            };

            return _connectionFactory.Create(config);
        }

        public IEventDispatcher GetDispatcher()
        {
            return new EventDispatcher(this);
        }

        public IEventEmitter GetEmitter()
        {
            return new EventEmitter(this, _observer);
        }
    }
}
