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
            this.RabbitMqUser = config.GetParameterAsString("RabbitMQ.User");
            this.RabbitMqPassword = config.GetParameterAsString("RabbitMQ.Password");
            this.ApiVersion = config.GetParameterAsString("SDRN.ApiVersion");
            this.ServerInstance = config.GetParameterAsString("SDRN.ServerInstance");
            this.ServerExchange = config.GetParameterAsString("SDRN.ServerExchange");
            this.DeviceExchange = config.GetParameterAsString("SDRN.DeviceExchange");
            this.ServerQueueNamePart = config.GetParameterAsString("SDRN.ServerQueueNamePart");
            this.DeviceQueueNamePart = config.GetParameterAsString("SDRN.DeviceQueueNamePart");

            var serverQueuesParam = config.GetParameterAsString("SDRN.ServerQueues");
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
        }

        public string RabbitMqHost { get; set; }

        public string RabbitMqUser { get; set; }

        public string RabbitMqPassword { get; set; }

        public string ApiVersion { get; set; }

        public string ServerInstance { get; set; }

        public string DeviceExchange { get; set; }
        public string ServerExchange { get; set; }

        public string ServerQueueNamePart { get; set; }

        public string DeviceQueueNamePart { get; set; }

        public IDictionary<string, ServerQueueDecriptor> ServerQueueus { get; set; }

    }
}
