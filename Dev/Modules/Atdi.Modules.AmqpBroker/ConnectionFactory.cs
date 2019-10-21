using System;

namespace Atdi.Modules.AmqpBroker
{
    public class ConnectionFactory
    {
        private readonly IBrokerObserver _brokerObserver;

        public ConnectionFactory(IBrokerObserver brokerObserver)
        {
            this._brokerObserver = brokerObserver;
            this._brokerObserver.Info("AmqpBroker.Initialization", "AMQP Broker Client starting ...", this);
        }

        public Connection Create(ConnectionConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return new Connection(config, _brokerObserver);
        }
    }
}
