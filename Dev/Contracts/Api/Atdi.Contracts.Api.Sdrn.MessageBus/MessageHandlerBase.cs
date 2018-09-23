using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.Sdrn.MessageBus
{
    public class MessageHandlerBase<TObject> : IMessageHandler<TObject>
    {
        public MessageHandlerBase(string messageType)
        {
            this.MessageType = messageType;
        }

        public string MessageType { get; }

        public void Handle(IReceivedMessage<TObject> message)
        {
            this.OnHandle(message);
        }

        public virtual void OnHandle(IReceivedMessage<TObject> message)
        {
            return;
        }
    }
}
