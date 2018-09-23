using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Modules.Sdrn.MessageBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal class MessagePublisher : IMessagePublisher
    {
        private readonly BusLogger _logger;
        private readonly EnvironmentDescriptor _environmentDescriptor;
        private readonly MessageConverter _messageConverter;
        private readonly RabbitMQBus _rabbitBus;

        internal MessagePublisher(EnvironmentDescriptor environmentDescriptor, MessageConverter messageConverter, BusLogger logger)
        {
            this._logger = logger;
            this._environmentDescriptor = environmentDescriptor;
            this._messageConverter = messageConverter;  
            this._rabbitBus = new RabbitMQBus("Publication", environmentDescriptor, logger);
        }

        public void Dispose()
        {
            this._rabbitBus.Dispose();
        }

        private ExchangeDescriptor DefineExchangeDescriptor(string messsgeType)
        {
            var exchangeDescriptor = new ExchangeDescriptor
            {
                RoutingKey = this._environmentDescriptor.GetRoutingKeyByMessageType(messsgeType),
                Exchange = this._environmentDescriptor.BuildDeviceExchangeName()
            };

            return exchangeDescriptor;
        }

        public IMessageToken Send<TObject>(string messageType, TObject messageObject, string correlationToken = null)
        {
            var exchange = this.DefineExchangeDescriptor(messageType);
            var message = this._messageConverter.Pack<TObject>(messageType, messageObject);
            message.CorrelationId = correlationToken;
            message.Headers = new Dictionary<string, object>
            {
                ["SdrnServer"] = this._environmentDescriptor.SdrnServerInstance,
                ["SensorName"] = this._environmentDescriptor.SdrnDeviceSensorName,
                ["SensorTechId"] = this._environmentDescriptor.SdrnDeviceSensorTechId,
                ["Created"] = DateTime.Now.ToString("o")
            };

            var messageId = this._rabbitBus.Publish(exchange.Exchange, exchange.RoutingKey, message);

            var token = new MessageToken(messageId, messageType);
            return token;
        }

        //private byte[] SerializeObjectToByteArray<TObject>(TObject source)
        //{
        //    if (source == null)
        //    {
        //        return new byte[] { };
        //    }

        //    byte[] result = null;

        //    var json = JsonConvert.SerializeObject(source);
        //    result = Encoding.UTF8.GetBytes(json);

        //    return result;
        //}
    }
}
