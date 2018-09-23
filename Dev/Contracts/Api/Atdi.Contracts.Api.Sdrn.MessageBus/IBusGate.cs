using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.Sdrn.MessageBus
{
    public interface IBusGate : IDisposable
    {
        IBusGateConfig Config { get; }

        IMessageDispatcher CreateDispatcher(string name, IBusEventObserver eventObserver = null);

        IMessagePublisher CreatePublisher(IBusEventObserver eventObserver = null);
    }
}
