using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class SdrnServerDescriptor
    {
        
        public SdrnServerDescriptor(IComponentConfig config)
        {
            this.RabbitMqHost = config.GetParameterAsString("RabbitMQ.Host");
            this.RabbitMqVirtualHost= config.GetParameterAsString("RabbitMQ.VirtualHost");
            this.RabbitMqUser = config.GetParameterAsString("RabbitMQ.User");
            this.RabbitMqPassword = config.GetParameterAsString("RabbitMQ.Password");
            this.ApiVersion = config.GetParameterAsString("SDRN.ApiVersion");
            this.ServerInstance = config.GetParameterAsString("SDRN.Server.Instance");
            this.ServerExchange = config.GetParameterAsString("SDRN.Server.Exchange");
            this.DeviceExchange = config.GetParameterAsString("SDRN.Device.Exchange");
            this.ServerQueueNamePart = config.GetParameterAsString("SDRN.Server.QueueNamePart");
            this.DeviceQueueNamePart = config.GetParameterAsString("SDRN.Device.QueueNamePart");

            var serverQueuesParam = config.GetParameterAsString("SDRN.Server.Queues");
            var serverQueues = new Dictionary<string, ServerQueueDecriptor>();

            if (!string.IsNullOrEmpty(serverQueuesParam))
            {
                var serverQueuesParts = serverQueuesParam.Split(new string[] { "; ", ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (serverQueuesParts.Length > 0)
                {
                    serverQueues = serverQueuesParts.Select(es => new ServerQueueDecriptor(es)).ToDictionary(k=> k.RoutingKey, v => v);
                }
            }
            this.ServerQueueus = serverQueues;

            this.UseEncryption = config.GetParameterAsBoolean("SDRN.MessageConvertor.UseEncryption");
            this.UseСompression = config.GetParameterAsBoolean("SDRN.MessageConvertor.UseСompression");
        }

        public string RabbitMqHost { get; set; }

        public string RabbitMqVirtualHost { get; set; }

        public string RabbitMqUser { get; set; }

        public string RabbitMqPassword { get; set; }

        public string ApiVersion { get; set; }

        public string ServerInstance { get; set; }

        public string DeviceExchange { get; set; }
        public string ServerExchange { get; set; }

        public string ServerQueueNamePart { get; set; }

        public string DeviceQueueNamePart { get; set; }

        public bool UseEncryption { get; set; }

        public bool UseСompression { get; set; }

        public IDictionary<string, ServerQueueDecriptor> ServerQueueus { get; set; }

    }
}
