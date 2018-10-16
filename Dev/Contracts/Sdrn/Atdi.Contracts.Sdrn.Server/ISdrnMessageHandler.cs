
using Atdi.DataModels.Sdrns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Server
{
    public interface ISdrnMessageHandler<TMessageType, TDeliveryObject>
        where TMessageType : SdrnBusMessageType<TDeliveryObject>, new()
    {
        void Handle(ISdrnIncomingEnvelope<TDeliveryObject> incomingEnvelope, ISdrnMessageHandlingResult result);
    }

}
