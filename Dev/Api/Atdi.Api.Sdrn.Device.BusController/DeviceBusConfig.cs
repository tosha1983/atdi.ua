using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.Modules.Sdrn.DeviceBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    

    internal sealed class BusBufferConfig 
    {
        public BusBufferConfig()
        {
            this.Type = BufferType.None;
        }

        public BufferType Type { get; set; }

        public string Folder { get; set; }

        public string ConnectionString { get; set; }

        public ContentType ContentType { get; set; }

        public override string ToString()
        {
            return $"Type = '{Type}', ContentType = '{ContentType}', OutboxFolder = '{Folder}'";
        }
    }

    internal class DeviceBusConfig
    {
        private string  _messagesBindingsValue;

        internal DeviceBusConfig(IBusGateConfig gateConfig)
        {
            this.GateConfig = gateConfig;
            this.OutboxBufferConfig = new BusBufferConfig();
            this.InboxBufferConfig = new BusBufferConfig();
        }

        public IBusGateConfig GateConfig { get; private set; }

        public string RabbitMqHost { get; set; }

        public int? RabbitMqPort { get; set; }

        public string RabbitMqVirtualHost { get; set; }

        public string RabbitMqUser { get; set; }

        public string RabbitMqPassword { get; set; }

        public string SdrnApiVersion { get; set; }

        public string SdrnServerInstance { get; set; }

        public string SdrnServerQueueNamePart { get; set; }

        public string SdrnDeviceSensorName { get; set; }

        public string SdrnDeviceSensorTechId { get; set; }

        public string SdrnDeviceExchange { get; set; }

        public string SdrnDeviceQueueNamePart { get; set; }

        public bool SdrnMessageConvertorUseEncryption { get; set; }

        public bool SdrnMessageConvertorUseCompression { get; set; }

        public ContentType DeviceBusContentType { get; set; }

        public string DeviceBusSharedSecretKey { get; set; }

        public string DeviceBusClient { get; set; }

        public string DeviceBusProtocol { get; set; } = "DeviceBus 3.0";

        public string SdrnDeviceMessagesBindings
        {
            get => this._messagesBindingsValue;
            set
            {
                this._messagesBindingsValue = value;

                var bindings = new Dictionary<string, MessagesBindingDescriptor>();

                if (!string.IsNullOrEmpty(this._messagesBindingsValue))
                {
                    var bindingParts = this._messagesBindingsValue.Split(new[] { "; ", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (bindingParts.Length > 0)
                    {
                        bindings = bindingParts.Select(es => new MessagesBindingDescriptor(es)).ToDictionary(k => k.MessageType, v => v);
                    }
                }
                this.MessagesBindings = bindings;
            }
        }

        public IDictionary<string, MessagesBindingDescriptor> MessagesBindings { get; private set; }

        public BusBufferConfig OutboxBufferConfig { get; }

        public BusBufferConfig InboxBufferConfig { get; }

        public override string ToString()
        {
            return $"Client='{this.DeviceBusClient}', Protocol='{DeviceBusProtocol}', ContentType={this.DeviceBusContentType}, Buffer={this.OutboxBufferConfig?.Type}, SdrnServer='{this.SdrnServerInstance}'";
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
