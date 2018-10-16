using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.AmqpBroker
{
    public class BusConnectionFactory
    {
        private readonly IBrokerObserver _brokerObserver;

        public BusConnectionFactory(IBrokerObserver brokerObserver)
        {
            this._brokerObserver = brokerObserver;
        }

        public BusConnection Create(BusConnectionConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return new BusConnection(config, _brokerObserver);
        }
    }
}
