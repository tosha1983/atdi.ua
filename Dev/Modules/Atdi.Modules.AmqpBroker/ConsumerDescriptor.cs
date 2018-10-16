using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.AmqpBroker
{
    internal class ConsumerDescriptor
    {
        public IDeliveryHandler Handler { get; set; }

        public string Tag { get; set; }

        public string Queue { get; set; }

        public Consumer Consumer { get; set; }
    }
}
