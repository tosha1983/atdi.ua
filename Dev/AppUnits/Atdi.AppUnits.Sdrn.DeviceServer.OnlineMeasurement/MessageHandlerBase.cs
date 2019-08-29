using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.DataModels.Sdrns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement
{
    public class MessageHandlerBase<TDeliveryObject, TMessageType> : MessageHandlerBase<TDeliveryObject>
        where TMessageType : SdrnBusMessageType<TDeliveryObject>, new()
    {
        public MessageHandlerBase() 
            : base((new TMessageType()).Name)
        {
        }
    }
}
