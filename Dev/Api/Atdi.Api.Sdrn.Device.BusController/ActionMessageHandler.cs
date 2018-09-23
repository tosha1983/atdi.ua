using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal class ActionMessageHandler<TObject> : IMessageHandler<TObject>
    {
        private readonly Action<IReceivedMessage<TObject>> action;

        internal ActionMessageHandler(string messageType, Action<IReceivedMessage<TObject>> action)
        {
            MessageType = messageType;
            this.action = action;
        }
        public string MessageType { get; }

        public void Handle(IReceivedMessage<TObject> message)
        {
            this.action(message);
        }
    }
}
