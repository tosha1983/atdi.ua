using System.Collections.Generic;

namespace Atdi.Modules.AmqpBroker
{
    public class DeliveryMessage : IDeliveryMessage
    {
        public string Id { get; set; }
        public string AppId { get; set; }
        public string Type { get; set; }
        public string ContentType { get; set; }
        public string ContentEncoding { get; set; }
        public string CorrelationId { get; set; }
        public IDictionary<string, object> Headers { get; set; }
        public byte[] Body { get; set; }
    }
}
