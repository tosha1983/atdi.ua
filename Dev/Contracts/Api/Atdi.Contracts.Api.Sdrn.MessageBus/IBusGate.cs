using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.Sdrn.MessageBus
{
    public interface IBusGate : IDisposable
    {
        string Tag { get; }

        IBusGateConfig Config { get; }

        IMessageDispatcher CreateDispatcher(string dispatcherTag, IBusEventObserver eventObserver = null);

        IMessagePublisher CreatePublisher(string publisherTag, IBusEventObserver eventObserver = null);
    }
}
