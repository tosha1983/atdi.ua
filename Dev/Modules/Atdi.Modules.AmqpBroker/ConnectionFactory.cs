using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.AmqpBroker
{
    public class ConnectionFactory
    {
        private readonly IBrokerObserver _brokerObserver;

        public ConnectionFactory(IBrokerObserver brokerObserver)
        {
            this._brokerObserver = brokerObserver;
            this._brokerObserver.Verbouse("AmqpBroker.ConnectionFactory", $"The connection factory is initialized successfully", this);
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
