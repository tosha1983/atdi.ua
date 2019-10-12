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
        private readonly EventSystemConfig _sysConfig;

        private readonly ISubscriberActivator _activator;
        private readonly IEventSystemObserver _observer;
        private readonly ConnectionFactory _connectionFactory;
        private Connection _emitterConnection;
        private Connection _dispatcherConnection;
        private Connection _declaratorConnection;

        public EventSite(IEventSiteConfig config, ISubscriberActivator activator, IEventSystemObserver observer)
        {
            this._config = config;
            this._sysConfig = new EventSystemConfig(_config);

            this._activator = activator;
            this._observer = observer;
            this._connectionFactory = new ConnectionFactory(new AmqpBrokerObserver(observer));
            

            using (var declarationChannel = this.DeclaratorConnection.CreateChannel())
            {
                declarationChannel.DeclareDurableDirectExchange(this._sysConfig.BuildEventExchangeName());
                declarationChannel.DeclareDurableQueue(this._sysConfig.BuildCommonLogQueueName());
            }

            this._observer.Info("EventSystem.Initialize", $"The event system initialized", this);
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
                            var c = CreateConnection("EventEmitter");
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

        internal EventSystemConfig SysConfig => _sysConfig;

        public IEventSiteConfig Config => _config;

        private Connection CreateConnection(string name)
        {
            var config = new ConnectionConfig
            {
                AutoRecovery = true,
                ConnectionName = $"EventSystem.[{_sysConfig.AppName}].[{name}].[v{_sysConfig.ApiVersion}]",
                HostName = _sysConfig.EventBusHost,
                VirtualHost = _sysConfig.EventBusVirtualHost,
                Port = _sysConfig.EventBusPort,
                UserName = _sysConfig.EventBusUser,
                Password = _sysConfig.EventBusPassword
            };

            return _connectionFactory.Create(config);
        }

        public IEventDispatcher GetDispatcher()
        {
            return new EventDispatcher(this, _observer);
        }

        public IEventEmitter GetEmitter()
        {
            return new EventEmitter(this, _observer);
        }
    }
}
