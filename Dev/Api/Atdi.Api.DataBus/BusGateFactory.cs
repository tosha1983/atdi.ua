using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.DataBus
{
    internal sealed class BusGateFactory : IBusGateFactory
    {
        public IBusConfig CreateConfig()
        {
            return new BusConfig();
        }

        public IBusGate CreateGate(IBusConfig config, IBusEventObserver eventObserver)
        {
            return new BusGate(config, eventObserver);
        }
    }
}
