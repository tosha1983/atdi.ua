using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Api.DataBus
{
    public interface IIncomingEnvelope
    {
        string From { get; }

        MessageToken Token { get; }

        DateTimeOffset Created { get; }
    }

    public interface IIncomingEnvelope<TMessageType, TDeliveryObject> : IIncomingEnvelope
        where TMessageType : IMessageType, new()
    {

        TMessageType Type { get; }
        
        TDeliveryObject DeliveryObject { get; }
    }
}
