using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client.Exceptions;
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
        private readonly Dictionary<string, ConsumerDescriptor> _consumers;
        private MessageSendingState _messageSendingState;

        internal Channel(Connection connection, IBrokerObserver logger)
        {
            this._connection = connection;
            this._logger = logger;
            this._consumers = new Dictionary<string, ConsumerDescriptor>();
        }

        internal RBC.IModel RealChannel => _channel;

        public Connection Connection => _connection;

        public int Number => _channelNumber;

        public bool IsOpen
        {
            get
            {
                if (_channel != null)
                {
                    return _channel.IsOpen;
                }
                return false;
            }
        }

        public override string ToString()
        {
            return $"Num=#{_channelNumber}, Opened={this.IsOpen}, State={_messageSendingState}";
        }

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

                this._channel = this._connection.CreateAmqpChannel();
                _channel.ConfirmSelect();
                _channel.BasicQos(0, 1, false);

                _channelNumber = _channel.ChannelNumber;

                this._channel.CallbackException += _channel_CallbackException;
                this._channel.ModelShutdown += _channel_ModelShutdown;
                this._channel.BasicAcks += _channel_BasicAcks;
                this._channel.BasicNacks += _channel_BasicNacks;
                this._channel.BasicReturn += _channel_BasicReturn;

                this._logger.Info("AmqpBroker.ChannelEstablishment", $"The channel is established successfully: {this}, {_connection}", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.EstablishChannelException, "AmqpBroker.ChannelEstablishment", e, this);
                throw new InvalidOperationException($"The channel is not established: {this}", e);
            }
        }

        private void _channel_BasicReturn(object sender, RBE.BasicReturnEventArgs e)
        {
            this._logger.Verbouse("AmqpBroker.ChannelEvent", $"The Basic Return Event occurred: {this}, ReplyText='{e.ReplyText}', RoutingKey='{e.RoutingKey}', Exchange='{e.Exchange}', MessageId='{e.BasicProperties.MessageId}'", this);
            if (this._messageSendingState == MessageSendingState.Sending)
            {
                this._messageSendingState = MessageSendingState.Returned;
            }
        }

        private void _channel_BasicNacks(object sender, RBE.BasicNackEventArgs e)
        {
            this._logger.Verbouse("AmqpBroker.ChannelEvent", $"The Basic Nacks Event occurred: {this}, DeliveryTag='{e.DeliveryTag}', Multiple='{e.Multiple}', Requeue='{e.Requeue}'", this);
            if (this._messageSendingState == MessageSendingState.Sending)
            {
                this._messageSendingState = MessageSendingState.Nacks;
            }
        }

        private void _channel_BasicAcks(object sender, RBE.BasicAckEventArgs e)
        {
            this._logger.Verbouse("AmqpBroker.ChannelEvent", $"The Basic Acks Event occurred: {this}, DeliveryTag='{e.DeliveryTag}', Multiple='{e.Multiple}'", this);
            if (this._messageSendingState == MessageSendingState.Sending)
            {
                this._messageSendingState = MessageSendingState.Sent;
            }
        }

        private void _channel_ModelShutdown(object sender, RBC.ShutdownEventArgs e)
        {
            this._logger.Verbouse("AmqpBroker.ChannelShutdown", $"The channel is shutdown: {this}, Initiato='{e.Initiator}', Reason='{e.ReplyText}', Code=#{e.ReplyCode}, {_connection}", this);
            if (this._messageSendingState == MessageSendingState.Sending)
            {
                this._messageSendingState = MessageSendingState.Aborted;
            }
        }

        private void _channel_CallbackException(object sender, RBE.CallbackExceptionEventArgs e)
        {
            this._logger.Verbouse("AmqpBroker.ChannelEvent", $"The Callback Exception Event occurred: {this}, Exception='{e.Exception.Message}', {this._connection}", this);
            this._logger.Exception(BrokerEvents.ExceptionEvent, "AmqpBroker.ChannelEvent", e.Exception, this);
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
                        this._logger.Info("AmqpBroker.ChannelClosing", $"The channel is closed successfully: {this}, {_connection}", this);
                    }
                }
                catch (Exception e)
                {
                    this._logger.Exception(BrokerEvents.CloseChannelException, "AmqpBroker.ChannelClosing", e, this);
                }
            }
        }

        private void DisposeChannel()
        {
            if (_channel != null)
            {
                try
                {
                    this._channel.CallbackException -= _channel_CallbackException;
                    this._channel.ModelShutdown -= _channel_ModelShutdown;
                    this._channel.BasicAcks -= _channel_BasicAcks;
                    this._channel.BasicNacks -= _channel_BasicNacks;
                    this._channel.BasicReturn -= _channel_BasicReturn;
                    _channel.Dispose();
                    this._logger.Verbouse("AmqpBroker.ChannelDisposing", $"The channel is disposed successfully: {this}, {_connection}", this);
                }
                catch (Exception e)
                {
                    this._logger.Exception(BrokerEvents.DisposeChannelException, "AmqpBroker.ChannelDisposing", e, this);
                }
                _channel = null;
            }
        }



        private void DetachConsumers()
        {
            if (_consumers != null && _consumers.Count > 0)
            {
                var descriptors = _consumers.Values.ToArray();
                foreach (var descriptor in descriptors)
                {
                    try
                    {
                        this.DetachConsumer(descriptor.Tag);
                    }
                    catch (Exception e)
                    {
                        this._logger.Exception(BrokerEvents.UnjoinConsumersException, "AmqpBroker.ConsumersDetaching", e, this);
                    }
                }
            }
        }

        public void Dispose()
        {
            this.DetachConsumers();
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

                this._logger.Verbouse("AmqpBroker.ExchangeDeclaration", $"The exchange is declared successfully: Exchange='{exchangeName}', {this}", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.DeclareExchangeException, "AmqpBroker.ExchangeDeclaration", e, this);
                throw new InvalidOperationException($"The exchange is not declared: Exchange='{exchangeName}', {this}", e);
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

                this._logger.Verbouse("AmqpBroker.QueueDeclaration", $"The queue and binding are declared successfully: Queue='{queue}', Routing='{routingKey}', Exchange='{exchange}', {this}", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.DeclareQueueException, "AmqpBroker.QueueDeclaration", e, this);
                throw new InvalidOperationException($"The queue and binding are not declared: Queue='{queue}', Routing='{routingKey}', Exchange='{exchange}', {this}", e);
            }
        }

        public void DeclareBinding(string queue, string exchange, string routingKey)
        {
            try
            {
                this.EstablishChannel();

                _channel.QueueBind(queue, exchange, routingKey, null);

                this._logger.Verbouse("AmqpBroker.BindingDeclaration", $"The binding is declared successfully: Queue='{queue}', Routing='{routingKey}', Exchange='{exchange}', {this}", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.DeclareQueueBindingException, "AmqpBroker.BindingDeclaration", e, this);
                throw new InvalidOperationException($"The binding is not declared: Queue='{queue}', Routing='{routingKey}', Exchange='{exchange}', {this}", e);
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
                this._logger.Verbouse("AmqpBroker.QueueDeclaration", $"The queue is declared successfully: Queue='{queueName}', {this}", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.DeclareQueueException, "AmqpBroker.DeclareQueue", e, this);
                throw new InvalidOperationException($"he queue is not declared successfully: Queue='{queueName}', {this}", e);
            }
        }

        private static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return Convert.ToInt64((TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
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


                this._logger.Verbouse("AmqpBroker.MessagePublication",
                    $"The message is published successfully: {this}, Type='{message.Type}', Exchange='{exchange}', Routing='{routingKey}', Id='{message.Id}'",
                    this);

                return message.Id;
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.PublishException, "AmqpBroker.MessagePublication", e, this);

                switch (this._messageSendingState)
                {
                    case MessageSendingState.Returned:
                        throw new InvalidOperationException($"The message is not published. It was returned by AMQP Broker: {this}, Type='{message?.Type}', Exchange='{exchange}', Routing='{routingKey}', Id='{message?.Id}'");
                    case MessageSendingState.Nacks:
                        throw new InvalidOperationException($"The message is not published. It was rejected by AMQP Broker: {this}, Type='{message?.Type}', Exchange='{exchange}', Routing='{routingKey}', Id='{message?.Id}'");
                    case MessageSendingState.Aborted:
                        throw new InvalidOperationException($"The message is not published. It sending was aborted: {this}, Type='{message?.Type}', Exchange='{exchange}', Routing='{routingKey}', Id='{message?.Id}'");
                    default:
                        throw new InvalidOperationException($"The message is not published. It was not sent: {this}, Type='{message?.Type}', Exchange='{exchange}', Routing='{routingKey}', Id='{message?.Id}'");
                }
            }
            finally
            {
                this._messageSendingState = MessageSendingState.None;
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

        public void AttachConsumer(string queue, string tag, IDeliveryHandler handler)
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

                this._logger.Verbouse("AmqpBroker.ConsumerAttachment", $"The consumer is attached successfully: {descriptor}, {this}", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.JoinConsumerException, "AmqpBroker.ConsumerAttachment", e, this);
                throw new InvalidOperationException($"The consumer is not attached fully: Tag='{tag}', Queue='{queue}', Handler='{handler?.GetType().FullName}', {this}", e);
            }
        }

        public void DetachConsumer(string tag)
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
                
                this._logger.Verbouse("AmqpBroker.ConsumerDetaching", $"The consumer is detached successfully: {descriptor}, {this}", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.UnjoinConsumerException, "AmqpBroker.ConsumerDetaching", e, this);
                throw new InvalidOperationException($"The consumer is not detached fully: Tag='{tag}', {this}", e);
            }
        }
    }
}
