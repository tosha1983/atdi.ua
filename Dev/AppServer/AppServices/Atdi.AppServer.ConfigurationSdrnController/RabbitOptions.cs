namespace Atdi.AppServer.ConfigurationSdrnController
{
    public class RabbitOptions
    {
        public string exchange { get; set; }
        public string queue { get; set; }
        public string routingKey { get; set; }
        public string nameConcumer { get; set; }
        public string NameSensor { get; set; }
        public string TechId { get; set; }

        public RabbitOptions(string exchange_, string routingKey_, string queue_, string nameConcumer_)
        {
            exchange = exchange_;
            routingKey = routingKey_;
            queue = queue_;
            nameConcumer = nameConcumer_;
        }

        public RabbitOptions(string exchange_, string routingKey_, string queue_, string NameSensor_, string TechId_)
        {
            exchange = exchange_;
            routingKey = routingKey_;
            queue = queue_;
            NameSensor = NameSensor_;
            TechId = TechId_;
        }

    }
}
