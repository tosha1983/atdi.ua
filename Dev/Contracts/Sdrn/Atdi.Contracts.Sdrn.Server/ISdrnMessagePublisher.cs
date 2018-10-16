using Atdi.DataModels.Sdrns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Server
{
    public interface ISdrnMessagePublisher : IDisposable
    {
        string Tag { get; }

        ISdrnOutgoingEnvelope<TMessageType, TDeliveryObject> CreateOutgoingEnvelope<TMessageType, TDeliveryObject>()
             where TMessageType : SdrnBusMessageType<TDeliveryObject>, new();

        string Send<TMessageType, TDeliveryObject>(ISdrnOutgoingEnvelope<TMessageType, TDeliveryObject> envelope)
            where TMessageType : SdrnBusMessageType<TDeliveryObject>, new();
    }
}
