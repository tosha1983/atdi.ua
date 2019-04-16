using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.DataBus;

namespace Atdi.Contracts.Api.DataBus
{
    public interface IMessageHandler
    {
        
    }
    public interface IMessageHandler<TMessageType, TDeliveryObject> : IMessageHandler
        where TMessageType : IMessageType, new()
    {
        void Handle(IIncomingEnvelope<TMessageType, TDeliveryObject> envelope, IHandlingResult result);
    }
}
