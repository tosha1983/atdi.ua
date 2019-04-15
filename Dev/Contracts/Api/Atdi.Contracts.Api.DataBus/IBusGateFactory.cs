using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.DataBus
{
    public interface IBusGateFactory
    {
        IBusConfig CreateConfig();

        IBusGate CreateGate(IBusConfig config, IBusEventObserver eventObserver);
    }
}
