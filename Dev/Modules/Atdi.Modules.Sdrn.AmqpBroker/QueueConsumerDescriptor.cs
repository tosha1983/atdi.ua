using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.AmqpBroker
{
    public class QueueConsumerDescriptor
    {
        public IMessageHandler Handler { get; set; }

        public string Tag { get; set; }

        public string Queue { get; set; }

        public QueueConsumer Consumer { get; set; }
    }
}
