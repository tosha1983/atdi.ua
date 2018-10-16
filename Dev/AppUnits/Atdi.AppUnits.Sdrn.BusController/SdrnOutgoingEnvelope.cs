using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class SdrnOutgoingEnvelope<TMessageType, TDeliveryObject> : ISdrnOutgoingEnvelope<TMessageType, TDeliveryObject>
        where TMessageType : SdrnBusMessageType<TDeliveryObject>, new()
    {
        public SdrnOutgoingEnvelope()
        {
            this.MessageType = new TMessageType();
        }

        public TMessageType MessageType { get; }

        public string CorrelationToken { get; set; }

        public string SensorName { get; set; }

        public string SensorTechId { get; set; }

        public TDeliveryObject DeliveryObject { get; set; }
    }
}
