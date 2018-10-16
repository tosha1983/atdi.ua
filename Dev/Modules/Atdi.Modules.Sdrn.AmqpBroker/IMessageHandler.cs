using Atdi.Modules.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.AmqpBroker
{
    public enum MessageHandlingResult
    {
        Ignore,
        Confirm,
        Reject
    }

    public interface IDeliveryContext
    {
        string ConsumerTag { get; }

        string DeliveryTag { get; }

        bool Redelivered { get; }

        string Exchange { get;  }

        string RoutingKey { get;  }
    }

    public interface IMessageHandler
    {
        MessageHandlingResult Handle(Message message, IDeliveryContext deliveryContext);
    }
}
