namespace Atdi.Modules.AmqpBroker
{
    internal class ConsumerDescriptor
    {
        public IDeliveryHandler Handler { get; set; }

        public string Tag { get; set; }

        public string Queue { get; set; }

        public Consumer Consumer { get; set; }

        public override string ToString()
        {
            return $"Tag='{this.Tag}', Queue='{this.Queue}', Handler='{this.Handler?.GetType().FullName}', AmqpTag='{this.Consumer?.ConsumerTag}'";
        }
    }
}
