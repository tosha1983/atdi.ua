using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RBC = RabbitMQ.Client;
using RBE = RabbitMQ.Client.Events;

namespace Atdi.Modules.AmqpBroker
{
    public class Channel : IDisposable
    {
        private enum MessageSendingState
        {
            None = 0,
            Sending,
            Returned,
            Nacks,
            Aborted,
            Sent
        }
        private readonly Connection _connection;
        private readonly IBrokerObserver _logger;
        private RBC.IModel _channel;
        private int _channelNumber;
        private Dictionary<string, ConsumerDescriptor> _consumers;
        private MessageSendingState _messageSendingState;

        internal Channel(Connection connection, IBrokerObserver logger)
        {
            this._connection = connection;
            this._logger = logger;
            this._consumers = new Dictionary<string, ConsumerDescriptor>();
        }

        internal RBC.IModel RealChannel { get => _channel; }

        public Connection Connection { get => _connection; }

        public int Number { get => _channelNumber; }

        public void EstablishChannel(bool onlyInstance = false)
        {
            try
            {
                if (_channel != null)
                {
                    
                    if (onlyInstance || _channel.IsOpen)
                    {
                        return;
                    }
                    this.DisposeChannel();
                }

                this._channel = this._connection.RealConnection.CreateModel();
                _channel.ConfirmSelect();
                _channel.BasicQos(0, 1, false);

                _channelNumber = _channel.ChannelNumber;

                this._channel.CallbackException += _channel_CallbackException;
                this._channel.ModelShutdown += _channel_ModelShutdown;
                this._channel.BasicAcks += _channel_BasicAcks;
                this._channel.BasicNacks += _channel_BasicNacks;
                this._channel.BasicReturn += _channel_BasicReturn;

                this._logger.Verbouse("RabbitMQ.EstablishChannel", $"The channel #{_channelNumber} is established successfully", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.EstablishChannelException, "RabbitMQ.EstablishChannel", e, this);
                throw new InvalidOperationException("The channel to RabbitMQ is not established", e);
            }
        }

        private void _channel_BasicReturn(object sender, RBE.BasicReturnEventArgs e)
        {
            this._logger.Verbouse("RabbitMQ.Channel.BasicReturn", $"The message was returned: ReplyText = '{e.ReplyText}', RoutingKey = '{e.RoutingKey}', Exchange = '{e.Exchange}', MessageId = '{e.BasicProperties.MessageId}'", this);
            if (this._messageSendingState == MessageSendingState.Sending)
            {
                this._messageSendingState = MessageSendingState.Returned;
            }
        }

        private void _channel_BasicNacks(object sender, RBE.BasicNackEventArgs e)
        {
            this._logger.Verbouse("RabbitMQ.Channel.BasicNacks", $"The message was Nacks: DeliveryTag = '{e.DeliveryTag}', Multiple = '{e.Multiple}', Requeue = '{e.Requeue}'", this);
            if (this._messageSendingState == MessageSendingState.Sending)
            {
                this._messageSendingState = MessageSendingState.Nacks;
            }
        }

        private void _channel_BasicAcks(object sender, RBE.BasicAckEventArgs e)
        {
            this._logger.Verbouse("RabbitMQ.Channel.BasicAcks", $"The message was Acks: DeliveryTag = '{e.DeliveryTag}', Multiple = '{e.Multiple}'", this);
            if (this._messageSendingState == MessageSendingState.Sending)
            {
                this._messageSendingState = MessageSendingState.Sent;
            }
        }

        private void _channel_ModelShutdown(object sender, RBC.ShutdownEventArgs e)
        {
            this._logger.Verbouse("RabbitMQ.ShutdownChannel", $"The channel #'{this._channelNumber}' is shutted down: Connection: '{_connection.Config.ConnectionName}', Initiator: '{e.Initiator}', Reasone: '{e.ReplyText}', Code: #{e.ReplyCode}", this);
            if (this._messageSendingState == MessageSendingState.Sending)
            {
                this._messageSendingState = MessageSendingState.Aborted;
            }
        }

        private void _channel_CallbackException(object sender, RBE.CallbackExceptionEventArgs e)
        {
            this._logger.Verbouse("RabbitMQ.CallbackExceptionChannel", $"The exception is occurred: Connection: '{this._connection.Config.ConnectionName}', Channel: #{_channelNumber}, Message: '{e.Exception.Message}'", this);
            this._logger.Exception(BrokerEvents.ExceptionEvent, "RabbitMQ.CallbackExceptionChannel", e.Exception, this);
        }

        private void CloseChannel(string reason)
        {
            if (_channel != null)
            {
                try
                {
                    if (_channel.IsOpen)
                    {
                        _channel.Close(200, reason);
                    }
                }
                catch (Exception e)
                {
                    this._logger.Exception(BrokerEvents.CloseChannelException, "RabbitMQ.CloseChannel", e, this);
                }
            }
        }

        private void DisposeChannel()
        {
            if (_channel != null)
            {
                try
                {
                    _channel.Dispose();
                }
                catch (Exception e)
                {
                    this._logger.Exception(BrokerEvents.DisposeChannelException, "RabbitMQ.DisposeChannel", e, this);
                }
                _channel = null;
            }
        }



        private void UnjoinConsumers()
        {
            if (_consumers != null && _consumers.Count > 0)
            {
                var descriptors = _consumers.Values.ToArray();
                foreach (var descriptor in descriptors)
                {
                    var consumer = descriptor.Consumer;
                    try
                    {
                        this.UnjoinConsumer(descriptor.Tag);
                    }
                    catch (Exception e)
                    {
                        this._logger.Exception(BrokerEvents.UnjoinConsumersException, "RabbitMQ.UnjoinConsumers", e, this);
                    }
                }
            }
        }

        public void Dispose()
        {
            this.UnjoinConsumers();
            this.CloseChannel("Channel.Dispose");
            this.DisposeChannel();
        }

        public void DeclareDurableDirectExchange(string exchangeName)
        {
            try
            {
                this.EstablishChannel();

                this._channel.ExchangeDeclare(
                        exchange: exchangeName,
                        type: "direct",
                        durable: true,
                        autoDelete: false,
                        arguments: null
                    );

                this._logger.Verbouse("RabbitMQ.DeclareExchange", $"The exchange with name '{exchangeName}' is declared successfully: Type = 'Direct', Durable: true", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.DeclareExchangeException, "RabbitMQ.DeclareExchange", e, this);
                throw new InvalidOperationException($"The exchange with name '{exchangeName}' is not declared", e);
            }
        }

        public void DeclareDurableQueue(string queue, string exchange, string routingKey)
        {
            try
            {
                this.EstablishChannel();

                _channel.QueueDeclare(
                           queue: queue,
                           durable: true,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

                _channel.QueueBind(queue, exchange, routingKey, null);

                this._logger.Verbouse("RabbitMQ.DeclareQueue", $"The queue with name '{queue}' is declared successfully: Routing key = '{routingKey}', Exchange name: '{exchange}'", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.DeclareQueueException, "RabbitMQ.DeclareQueue", e, this);
                throw new InvalidOperationException($"The queue with name '{queue}' is not declared", e);
            }
        }

        public void DeclareBinding(string queue, string exchange, string routingKey)
        {
            try
            {
                this.EstablishChannel();

                _channel.QueueBind(queue, exchange, routingKey, null);

                this._logger.Verbouse("RabbitMQ.DeclareBinding", $"The queue with name '{queue}' is declared successfully: Routing key = '{routingKey}', Exchange name: '{exchange}'", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.DeclareQueueBindingException, "RabbitMQ.DeclareBinding", e, this);
                throw new InvalidOperationException($"The queue binding is not declared: Queue name ='{queue}', Routing key = '{routingKey}', Exchange name = '{exchange}'", e);
            }
        }

        public void DeclareDurableQueue(string queueName)
        {
            try
            {
                this.EstablishChannel();

                _channel.QueueDeclare(
                           queue: queueName,
                           durable: true,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

                this._logger.Verbouse("RabbitMQ.DeclareQueue", $"The queue with name '{queueName}' is declared successfully", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.DeclareQueueException, "RabbitMQ.DeclareQueue", e, this);
                throw new InvalidOperationException($"The queue with name '{queueName}' is not declared", e);
            }
        }

        private static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return Convert.ToInt64((TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds);
        }

        public IDeliveryMessage CreateMessage()
        {
            return new DeliveryMessage();
        }

        public string Publish(string exchange, string routingKey, IDeliveryMessage message)
        {
            try
            {
                this.EstablishChannel();

                var props = this._channel.CreateBasicProperties();
                props.Persistent = true;
                if (!string.IsNullOrEmpty(message.Id))
                {
                    props.MessageId = message.Id;
                }
                if (!string.IsNullOrEmpty(message.AppId))
                {
                    props.AppId = message.AppId;
                }
                if (!string.IsNullOrEmpty(message.Type))
                {
                    props.Type = message.Type;
                }
                if (!string.IsNullOrEmpty(message.ContentType))
                {
                    props.ContentType = message.ContentType;
                }
                if (!string.IsNullOrEmpty(message.ContentEncoding))
                {
                    props.ContentEncoding = message.ContentEncoding;
                }
                if (!string.IsNullOrEmpty(message.CorrelationId))
                {
                    props.CorrelationId = message.CorrelationId;
                }
                props.Timestamp = new RBC.AmqpTimestamp(DateTimeToUnixTimestamp(DateTime.Now));
                props.Headers = message.Headers;

                this._messageSendingState = MessageSendingState.Sending;
                this._channel.BasicPublish(exchange, routingKey, false, props, message.Body);
                this._channel.WaitForConfirmsOrDie();
                if (this._messageSendingState != MessageSendingState.Sent)
                {
                    switch (this._messageSendingState)
                    {
                        case MessageSendingState.Returned:
                            throw new InvalidOperationException("The message was returned");
                        case MessageSendingState.Nacks:
                            throw new InvalidOperationException("The message was rejected");
                        case MessageSendingState.Aborted:
                            throw new InvalidOperationException("The message sending was aborted");
                        default:
                            throw new InvalidOperationException("The message was not sent");
                    }
                }

                this._logger.Verbouse("RabbitMQ.PublisheMessage", $"The message '{message.Type}' is published successfully: Exchange: '{exchange}', Routing= '{routingKey}', Id = '{message.Id}'", this);

                return message.Id;
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.PublishException, "RabbitMQ.PublisheMessage", e, this);
                throw new InvalidOperationException($"The message with type '{message.Type}' is not published", e);
            }
        }

        public void PackDeliveryObject(IDeliveryMessage message, object deliveryObject, bool useCompression, bool useEncryption)
        {
            var json = JsonConvert.SerializeObject(deliveryObject);
            var encoding = new Stack<string>();
            if (useCompression)
            {
                encoding.Push("compressed");
                json = Compressor.Compress(json);
            }
            if (useEncryption)
            {
                encoding.Push("encrypted");
                json = Encryptor.Encrypt(json);
            }
            message.Body = Encoding.UTF8.GetBytes(json);
            message.ContentEncoding = string.Join(", ", encoding.ToArray());
        }

        public object UnpackDeliveryObject(IDeliveryMessage message, Type deliveryObjectType)
        {
            var json = Encoding.UTF8.GetString(message.Body);
            if (!string.IsNullOrEmpty(message.ContentEncoding))
            {
                if (message.ContentEncoding.Contains("encrypted"))
                {
                    json = Encryptor.Decrypt(json);
                }
                if (message.ContentEncoding.Contains("compressed"))
                {
                    json = Compressor.Decompress(json);
                }
            }
            var deliveryObject = JsonConvert.DeserializeObject(json, deliveryObjectType);
            return deliveryObject;
        }

        public void JoinConsumer(string queue, string tag, IDeliveryHandler handler)
        {
            try
            {
                this.EstablishChannel(true);

                if (_consumers.ContainsKey(tag))
                {
                    return;
                }

                var descriptor = new ConsumerDescriptor
                {
                    Queue = queue,
                    Tag = tag,
                    Handler = handler,
                    Consumer = new Consumer(queue, tag, this, handler)
                };
                this._consumers.Add(tag, descriptor);
                _channel.BasicConsume(queue, false, tag, false, false, null, descriptor.Consumer);

                this._logger.Verbouse("RabbitMQ.JoinConsumer", $"The consumer '{tag}' is joined successfully: Channel: #{_channelNumber}, Queue: '{queue}', Handler: '{handler.GetType().FullName}'", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.JoinConsumerException, "RabbitMQ.JoinConsumer", e, this);
                throw new InvalidOperationException($"The consumer '{tag}' is not joined fully", e);
            }
        }

        public void UnjoinConsumer(string tag)
        {
            try
            {
                this.EstablishChannel(true);

                if (!_consumers.ContainsKey(tag))
                {
                    return;
                }

                var descriptor = this._consumers[tag];
                
                if (descriptor.Consumer.IsRunning)
                {
                    this._channel.BasicCancel(tag);
                }
                this._consumers.Remove(tag);
                
                this._logger.Verbouse("RabbitMQ.UnjoinConsumer", $"The consumer '{tag}' is unjoined successfully: Channel: #{_channelNumber}, Queue: '{descriptor.Queue}', Handler: '{descriptor.Handler.GetType().FullName}'", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.UnjoinConsumerException, "RabbitMQ.UnjoinConsumer", e, this);
                throw new InvalidOperationException($"The consumer '{tag}' is not unjoined fully", e);
            }
        }
    }
}
