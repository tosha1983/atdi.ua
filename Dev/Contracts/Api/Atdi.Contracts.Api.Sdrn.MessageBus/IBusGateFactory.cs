using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.Sdrn.MessageBus
{
    public interface IBusGateFactory
    {
        IBusGateConfig CreateConfig();

        IBusGate CreateGate(string gateTag, IBusGateConfig gateConfig, IBusEventObserver eventObserver = null);
    }
}
