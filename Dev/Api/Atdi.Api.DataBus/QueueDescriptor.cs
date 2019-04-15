using Atdi.Modules.AmqpBroker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.DataBus
{
    internal sealed class QueueDescriptor
    {
        public QueueDescriptor(string name, AmqpDeliveryHandler deliveryHandler)
        {
            this.Name = name;
            this.DeliveryHandler = deliveryHandler;
        }

        public string Name { get; }

        public AmqpDeliveryHandler DeliveryHandler { get; }
    }
}
