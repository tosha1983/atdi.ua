using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.Sdrn.MessageBus
{
    public interface IMessageDispatcher : IDisposable
    {
        MessageDispatcherState State { get; }

        void Activate();

        void Deactivate();

        void RegistryHandler<TObject>(string messageType, Action<IReceivedMessage<TObject>> handler);

        void RegistryHandler<TObject>(IMessageHandler<TObject> handler);
    }
}
