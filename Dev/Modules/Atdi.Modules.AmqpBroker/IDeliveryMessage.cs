using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.AmqpBroker
{
    public interface IDeliveryMessage
    {
        string Id { get; set; }

        string AppId { get; set; }

        string Type { get; set; }

        string ContentType { get; set; }

        string ContentEncoding { get; set; }

        string CorrelationId { get; set; }

        IDictionary<string, object> Headers { get; set; } 

        byte[] Body { get; set; }
    }
}
