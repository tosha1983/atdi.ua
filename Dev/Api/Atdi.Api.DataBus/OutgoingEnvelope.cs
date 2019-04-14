using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.DataBus
{
    internal sealed class OutgoingEnvelope<TMessageType, TDeliveryObject> : IOutgoingEnvelope<TMessageType, TDeliveryObject>
        where TMessageType : IMessageType, new()
    {
        public OutgoingEnvelope(IBusConfig config)
        {
            this.Created = DateTimeOffset.Now;
            this.Type = new TMessageType();
            this.Token = new MessageToken(this.Type.Name);
            this.ContentType = config.ContentType;
            this.Options = SendingOptions.Default;
            if (config.UseCompression.HasValue && config.UseCompression.Value)
            {
                this.Options |= SendingOptions.UseCompression;
            }
            if (config.UseEncryption.HasValue && config.UseEncryption.Value)
            {
                this.Options |= SendingOptions.UseEncryption;
            }
            if (config.Buffer != null && config.Buffer.Type != BufferType.None)
            {
                this.Options |= SendingOptions.UseBuffer;
            }
        }

        public string To { get; set; }

        public MessageToken Token { get; }

        public TMessageType Type { get; }

        public DateTimeOffset Created { get; }
   
        public TDeliveryObject DeliveryObject { get; set; }

        public SendingOptions Options { get; set; }

        public ContentType ContentType { get; set; }

        public override string ToString()
        {
            return $"Token: '{this.Token}', Type: '{typeof(TMessageType).FullName}', DeliveryObject: '{typeof(TDeliveryObject).FullName}'";
        }
    }
}
