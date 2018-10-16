using Atdi.Contracts.Sdrn.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class SdrnIncomingEnvelopeProperties
    {
        public string MessageId { get; set; }

        public string MessageType { get; set; }

        public DateTime Created { get; set; }

        public string CorrelationToken { get; set; }

        public string SensorName { get; set; }

        public string SensorTechId { get; set; }

    }

    public class SdrnIncomingEnvelope<TDeliveryObject> : ISdrnIncomingEnvelope<TDeliveryObject>
    {
        private readonly SdrnIncomingEnvelopeProperties properties;
        private readonly TDeliveryObject _deliveryObject;

        public SdrnIncomingEnvelope(SdrnIncomingEnvelopeProperties properties, TDeliveryObject deliveryObject)
        {
            this.properties = properties;
            this._deliveryObject = deliveryObject;
        }

        public string MessageId => properties.MessageId;

        public string MessageType => properties.MessageType;

        public DateTime Created => properties.Created;

        public string CorrelationToken { get => properties.MessageId; set { } }

        public string SensorName { get => properties.SensorName; set { } }

        public string SensorTechId { get => properties.SensorTechId; set { } }

        public TDeliveryObject DeliveryObject { get => _deliveryObject; set { } }
    }
}
