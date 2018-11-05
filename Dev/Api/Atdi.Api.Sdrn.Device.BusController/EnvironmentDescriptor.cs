using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal class EnvironmentDescriptor
    {
        private string  _messagesBindingsValue;
        internal EnvironmentDescriptor(IBusGateConfig gateConfig)
        {
            this.GateConfig = gateConfig;
        }

        public IBusGateConfig GateConfig { get; private set; }

        public string RabbitMQHost { get; set; }
        public string RabbitMQVirtualHost { get; set; }
        public string RabbitMQUser { get; set; }
        public string RabbitMQPassword { get; set; }
        public string SdrnApiVersion { get; set; }
        public string SdrnServerInstance { get; set; }
        public string SdrnServerQueueNamePart { get; set; }
        public string SdrnDeviceSensorName { get; set; }
        public string SdrnDeviceSensorTechId { get; set; }
        public string SdrnDeviceExchange { get; set; }
        public string SdrnDeviceQueueNamePart { get; set; }

        public bool SdrnMessageConvertorUseEncryption { get; set; }
        public bool SdrnMessageConvertorUseСompression { get; set; }

        public string SdrnDeviceMessagesBindings
        {
            get
            {
                return this._messagesBindingsValue;
            }
            set
            {
                this._messagesBindingsValue = value;

                var bindings = new Dictionary<string, MessagesBindingDecriptor>();

                if (!string.IsNullOrEmpty(this._messagesBindingsValue))
                {
                    var bindingParts = this._messagesBindingsValue.Split(new string[] { "; ", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (bindingParts.Length > 0)
                    {
                        bindings = bindingParts.Select(es => new MessagesBindingDecriptor(es)).ToDictionary(k => k.MessageType, v => v);
                    }
                }
                this.MessagesBindings = bindings;
            }
        }

        public IDictionary<string, MessagesBindingDecriptor> MessagesBindings { get; set; }


        public override string ToString()
        {
            return $"{this.SdrnApiVersion}: {this.SdrnDeviceSensorName} <=({this.RabbitMQHost})=> {this.SdrnServerInstance}";
        }

        public string BuildDeviceExchangeName()
        {
            return $"{this.SdrnDeviceExchange}.[v{this.SdrnApiVersion}]";
        }
        //ServerQueueName
        public string BuildServerQueueName(string routingKey)
        {
            return $"{this.SdrnServerQueueNamePart}.[{this.SdrnServerInstance}].[{routingKey}].[v{this.SdrnApiVersion}]";
        }

        public string BuildDeviceQueueName()
        {
            return $"{this.SdrnDeviceQueueNamePart}.[{this.SdrnDeviceSensorName}].[{this.SdrnDeviceSensorTechId}].[v{this.SdrnApiVersion}]";
        }

        public string GetRoutingKeyByMessageType(string messageType)
        {
            if (!this.MessagesBindings.ContainsKey(messageType))
            {
                throw new InvalidOperationException($"Unknown message type with name '{messageType}'");
            }

            var routingKey = this.MessagesBindings[messageType].RoutingKey;
            return $"[{this.SdrnServerInstance}].[{routingKey}]";
        }

        public string GetRoutingKeyByContext(string context)
        {
            return $"[{this.SdrnServerInstance}].[{context}]";
        }
    }
}
