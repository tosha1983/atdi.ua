using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.AmqpBroker
{
    public interface IDeliveryContext
    {
        string ConsumerTag { get; }

        string DeliveryTag { get; }

        bool Redelivered { get; }

        string Exchange { get; }

        string RoutingKey { get; }

        Channel Channel { get; }
    }
}
