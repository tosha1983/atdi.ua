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

        string Tag { get; }

        void Activate();

        void Deactivate();

        void RegistryHandler<TObject>(string messageType, Action<IReceivedMessage<TObject>> handler);

        void RegistryHandler<TObject>(IMessageHandler<TObject> handler);

        void TryAckMessage(IMessageToken token);

        IReceivedMessage<TObject> TryGetObject<TObject>(string messageType, string correlationId = null, bool isAutoAck = false);

        IReceivedMessage<TObject> WaitObject<TObject>(string messageType, string correlationId = null);
    }
}
