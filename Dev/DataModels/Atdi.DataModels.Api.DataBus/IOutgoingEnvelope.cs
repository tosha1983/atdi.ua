using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Api.DataBus
{
    [Flags]
    public enum SendingOptions
    {
        Default = 0,
        UseBuffer = 1,
        UseEncryption = 2,
        UseCompression = 4
    }

    public interface IOutgoingEnvelope<TMessageType, TDeliveryObject>
        where TMessageType : IMessageType, new()
    {
        string To { get; set; }

        MessageToken Token { get; }

        TMessageType Type { get; }

        DateTimeOffset Created { get; }

        TDeliveryObject DeliveryObject { get; set; }

        SendingOptions Options { get; set; }

        ContentType ContentType { get; set; }
    }
}
