using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.AmqpBroker
{
    public class Consumer : DefaultBasicConsumer, IDisposable
    {
        private class DeliveryContext : IDeliveryContext
        {
            public string ConsumerTag { get; set; }

            public string DeliveryTag { get; set; }

            public bool Redelivered { get; set; }

            public string Exchange  { get; set; }

            public string RoutingKey { get; set; }

            public Channel Channel { get; set; }
        }

        private readonly string _queue;
        private readonly string _tag;
        private readonly Channel _channel;
        private IDeliveryHandler _handler;


        public Consumer(string queue, string tag, Channel channel, IDeliveryHandler handler)
        {
            if (string.IsNullOrEmpty(queue))
            {
                throw new ArgumentNullException(nameof(queue));
            }

            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException(nameof(tag));
            }

            this._queue = queue;
            this._tag = tag;
            this._channel = channel ?? throw new ArgumentNullException(nameof(channel));
            this._handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public void Dispose()
        {
            if (_handler != null)
            {
                _handler = null;
            }
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            var message = new DeliveryMessage
            {
                Body = body,
                Headers = properties.Headers
            };

            if (properties.IsMessageIdPresent())
            {
                message.Id = properties.MessageId;
            }
            if (properties.IsAppIdPresent())
            {
                message.AppId = properties.AppId;
            }
            if (properties.IsTypePresent())
            {
                message.Type = properties.Type;
            }
            if (properties.IsContentTypePresent())
            {
                message.ContentType = properties.ContentType;
            }
            if (properties.IsContentEncodingPresent())
            {
                message.ContentEncoding = properties.ContentEncoding;
            }
            if (properties.IsCorrelationIdPresent())
            {
                message.CorrelationId = properties.CorrelationId;
            }

            var context = new DeliveryContext
            {
                ConsumerTag = consumerTag,
                DeliveryTag = deliveryTag.ToString(),
                Exchange = exchange,
                Redelivered = redelivered,
                RoutingKey = routingKey,
                Channel = this._channel
            };

            var result = this._handler.Handle(message, context);

            if (result == HandlingResult.Confirm)
            {
                this._channel.RealChannel.BasicAck(deliveryTag, false);
            }
            else if(result == HandlingResult.Reject)
            {
                this._channel.RealChannel.BasicReject(deliveryTag, false);
            }
        }
    }
}
