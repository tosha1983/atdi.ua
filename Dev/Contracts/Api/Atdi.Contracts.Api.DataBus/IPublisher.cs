using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.DataBus
{
    public interface IPublisher : IDisposable
    {
        IOutgoingEnvelope<TMessageType, TDeliveryObject> CreateEnvelope<TMessageType, TDeliveryObject>()
            where TMessageType : IMessageType, new();

        void Send<TMessageType, TDeliveryObject>(IOutgoingEnvelope<TMessageType, TDeliveryObject> envelope)
            where TMessageType : IMessageType, new();
    }
}
