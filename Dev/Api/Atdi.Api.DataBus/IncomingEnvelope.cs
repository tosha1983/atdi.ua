using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.DataBus
{
    internal sealed class IncomingEnvelope<TMessageType, TDeliveryObject> : IIncomingEnvelope<TMessageType, TDeliveryObject>
        where TMessageType : IMessageType, new()
    {

        public IncomingEnvelope(Type MessageTypeType, TDeliveryObject deliveryObject, string from, MessageToken messageToken, DateTimeOffset created)
        {
            this.Type = (TMessageType)Activator.CreateInstance(MessageTypeType);
            this.DeliveryObject = deliveryObject;
            this.From = from;
            this.Token = messageToken;
            this.Created = created;
        }
        public TMessageType Type { get; set; }

        public TDeliveryObject DeliveryObject { get; set; }

        public string From { get; set; }

        public MessageToken Token { get; set; }

        public DateTimeOffset Created { get; set; }
    }
}
