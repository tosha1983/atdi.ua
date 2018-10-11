using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Server
{
    public abstract class SdrnPrimaryHandlerBase<TObject> : IMessageHandler<TObject>
    {
        public SdrnPrimaryHandlerBase(string messageType)
        {
            this.MessageType = messageType;
        }

        public string MessageType { get; }

        public void Handle(IReceivedMessage<TObject> message)
        {
            this.OnHandle(message as ISdrnReceivedMessage<TObject>);
        }

        public abstract void OnHandle(ISdrnReceivedMessage<TObject> message);
    }
}
