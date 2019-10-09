namespace Atdi.Modules.AmqpBroker
{
    public enum HandlingResult
    {
        Ignore,
        Confirm,
        Reject
    }

    public interface IDeliveryHandler
    {
        HandlingResult Handle(IDeliveryMessage message, IDeliveryContext deliveryContext);
    }
}
