using Atdi.Modules.Sdrn.MessageBus;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.AmqpBroker
{
    public class QueueConsumer : DefaultBasicConsumer, IDisposable
    {
        private class DeliveryContext : IDeliveryContext
        {
            public string ConsumerTag { get; set; }

            public string DeliveryTag { get; set; }

            public bool Redelivered { get; set; }

            public string Exchange  { get; set; }

            public string RoutingKey { get; set; }
        }

        private readonly string _queue;
        private readonly string _tag;
        private IModel _channel;
        private IMessageHandler _handler;
        private bool _isJoined;

        public QueueConsumer(string queue, string tag, IModel channel, IMessageHandler handler)
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
            this._isJoined = false;
        }

        public void Join()
        {
            if (_isJoined)
            {
                return;
            }

            if (_channel == null)
            {
                throw new InvalidOperationException("The channel wasn't defined");
            }

            if (_channel.IsClosed)
            {
                throw new InvalidOperationException("The channel was closed");
            }

            _channel.BasicConsume(this._queue, false, this._tag, this);
            this._isJoined = true;
        }

        public void Unjoin()
        {
            if (!_isJoined)
            {
                return;
            }

            if (_channel == null)
            {
                throw new InvalidOperationException("The channel wasn't defined");
            }

            if (_channel.IsClosed)
            {
                throw new InvalidOperationException("The channel was closed");
            }

            _channel.BasicCancel(this._tag);
            this._isJoined = false;
        }

        public void ReJoin(IModel channel)
        {
            if (channel == null)
            {
                throw new ArgumentNullException(nameof(channel));
            }

            if (_channel != null)
            {
                if (_channel.IsOpen)
                {
                    if (_isJoined)
                    {
                        _channel.BasicCancel(this._tag);
                        _isJoined = false;
                    }
                    _channel.Close();
                }
                _channel.Dispose();
            }

            _channel = channel;

            this.Join();
        }

        public bool IsJoined => _isJoined;

        public void Dispose()
        {
            if (_channel != null)
            {
                if (_channel.IsOpen)
                {
                    if (_isJoined)
                    {
                        _channel.BasicCancel(this._tag);
                        _isJoined = false;
                    }
                    _channel.Close();
                }
                _channel.Dispose();
                _channel = null;
            }
            if (_handler != null)
            {
                _handler = null;
            }
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            var message = new Message
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
                RoutingKey = routingKey
            };

            var result = this._handler.Handle(message, context);

            if (result == MessageHandlingResult.Confirm)
            {
                this._channel.BasicAck(deliveryTag, false);
            }
            else if(result == MessageHandlingResult.Reject)
            {
                this._channel.BasicReject(deliveryTag, false);
            }
        }
    }
}
