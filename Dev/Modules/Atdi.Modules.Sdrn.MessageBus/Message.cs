using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.MessageBus
{
    public class Message
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string ContentType { get; set; }

        public string ContentEncoding { get; set; }

        public string CorrelationId { get; set; }

        public IDictionary<string, object> Headers { get; set; } 

        public byte[] Body { get; set; }
    }
}
