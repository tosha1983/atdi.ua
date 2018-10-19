using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WcfServices.Sdrn.Device
{
    public class SdrnServerDescriptor
    {
        public class QueueBindingDecriptor
        {
            public QueueBindingDecriptor(string bindingPart)
            {
                if (string.IsNullOrEmpty(bindingPart))
                {
                    throw new ArgumentNullException(nameof(bindingPart));
                }
                var value = bindingPart;
                if (value.StartsWith("{"))
                {
                    value = value.Substring(1);
                }
                if (value.EndsWith("}"))
                {
                    value = value.Substring(0, value.Length - 1);
                }

                var parts = value.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                {
                    throw new ArgumentException($"Incorrect binding part string '{bindingPart}'");
                }

                foreach (var part in parts)
                {
                    var attrs = part.Split(new string[] { "=", " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (attrs.Length != 2)
                    {
                        throw new ArgumentException($"Incorrect binding part string '{bindingPart}' in part '{part}'");
                    }

                    if ("messageType".Equals(attrs[0], StringComparison.OrdinalIgnoreCase))
                    {
                        this.MessageType = attrs[1];
                    }
                    else if ("routingKey".Equals(attrs[0], StringComparison.OrdinalIgnoreCase))
                    {
                        this.RoutingKey = attrs[1];
                    }
                }
            }

            public string MessageType { get; set; }

            public string RoutingKey { get; set; }
        }
        public SdrnServerDescriptor(IComponentConfig config, string instance)
        {
            this.Instance = instance; // config.GetParameterAsString("Instance");
            this.RabbitMqHost = config.GetParameterAsString("RabbitMQ.Host");
            this.RabbitMqVirtualHost = config.GetParameterAsString("RabbitMQ.VirtualHost");
            this.RabbitMqUser = config.GetParameterAsString("RabbitMQ.User");
            this.RabbitMqPassword = config.GetParameterAsDecodeString("RabbitMQ.Password", "Atdi.WcfServices.Sdrn.Device");
            this.ApiVersion = config.GetParameterAsString("SDRN.ApiVersion");
            this.ServerInstance = config.GetParameterAsString("SDRN.ServerInstance");
            this.MessagesExchange = config.GetParameterAsString("SDRN.MessagesExchange");
            this.ServerQueueNamePart = config.GetParameterAsString("SDRN.ServerQueueNamePart");
            this.DeviceQueueNamePart = config.GetParameterAsString("SDRN.DeviceQueueNamePart");

            this.AllowedSensors = new Dictionary<string, string>();
            this.AllowedSensors[instance] = instance;

            var bindingsParam = config.GetParameterAsString("SDRN.MessagesBindings");
            var bindings = new Dictionary<string, QueueBindingDecriptor>();

            if (!string.IsNullOrEmpty(bindingsParam))
            {
                var bindingParts = bindingsParam.Split(new string[] { "; ", ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (bindingParts.Length > 0)
                {
                    bindings = bindingParts.Select(es => new QueueBindingDecriptor(es)).ToDictionary(k=> k.MessageType, v => v);
                }
            }
            this.QueueBindings = bindings;
        }

        public string Instance { get; set; }

        public string RabbitMqHost { get; set; }

        public string RabbitMqVirtualHost { get; set; }


        public string RabbitMqUser { get; set; }

        public string RabbitMqPassword { get; set; }

        public string ApiVersion { get; set; }

        public string ServerInstance { get; set; }

        public string MessagesExchange { get; set; }

        public string ServerQueueNamePart { get; set; }

        public string DeviceQueueNamePart { get; set; }

        public IDictionary<string, QueueBindingDecriptor> QueueBindings { get; set; }

        public IDictionary<string, string> AllowedSensors { get; set; }

    }
}
