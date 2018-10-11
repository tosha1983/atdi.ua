using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.Sdrn.MessageBus
{
    public interface IMessageHandler<TObject>
    {
        string MessageType { get; }

        void Handle(IReceivedMessage<TObject> message);
    }


}
