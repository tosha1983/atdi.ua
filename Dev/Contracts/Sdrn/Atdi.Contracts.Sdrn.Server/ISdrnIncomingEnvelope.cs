using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Server
{
    public interface ISdrnIncomingEnvelope<TDeliveryObject> : ISdrnEnvelope<TDeliveryObject>
    {
        string MessageId { get; }

        string MessageType { get; }

        DateTime Created { get; }
    }
}
