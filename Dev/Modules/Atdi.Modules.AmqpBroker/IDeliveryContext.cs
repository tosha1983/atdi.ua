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

        string Queue { get; }
    }
}
