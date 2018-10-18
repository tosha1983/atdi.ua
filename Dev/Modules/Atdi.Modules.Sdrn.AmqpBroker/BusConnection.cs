using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Atdi.Modules.Sdrn.MessageBus;

namespace Atdi.Modules.Sdrn.AmqpBroker
{
    public class BusConnection : IDisposable
    {
        private readonly BusConnectionConfig _config;
        private readonly IBrokerObserver _logger;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _sharedChannel;
        private List<QueueConsumerDescriptor> _consumers;

        internal BusConnection(BusConnectionConfig config, IBrokerObserver logger)
        {
            this._config = config ?? throw new ArgumentNullException(nameof(config));
            this._logger = logger;

            this._consumers = new List<QueueConsumerDescriptor>();
            this._connectionFactory = new ConnectionFactory()
            {
                HostName = this._config.HostName,
                UserName = this._config.UserName,
                Password = this._config.Password,
            };
            if (!string.IsNullOrEmpty(this._config.VirtualHost))
            {
                this._connectionFactory.VirtualHost = this._config.VirtualHost;
            }
            if (this._config.AutoRecovery.HasValue)
            {
                this._connectionFactory.AutomaticRecoveryEnabled = this._config.AutoRecovery.Value;
            }
            if (this._config.Port.HasValue)
            {
                this._connectionFactory.Port = this._config.Port.Value;
            }

            this.EstablishConnection();
        }

        private void RestoreConsumers()
        {
            if (_consumers != null && _consumers.Count > 0 )
            {
                foreach (var consumerDescriptor in _consumers)
                {
                    var channel = this._connection.CreateModel();
                    consumerDescriptor.Consumer = new QueueConsumer(consumerDescriptor.Queue, consumerDescriptor.Tag, channel, consumerDescriptor.Handler);
                    consumerDescriptor.Consumer.Join();

                    this._logger.Verbouse("SDRN.RabbitMQ.RestoreConsumer", $"The consumer '{consumerDescriptor.Tag}' was restored: Queueu: '{consumerDescriptor.Queue}'", this);
                }
                this._logger.Verbouse("SDRN.RabbitMQ.RestoreConsumers", $"The consumers were restored", this);
            }
        }

        public void EstablishConnection()
        {
            if (this._connection != null)
            {
                if (this._connection.IsOpen)
                {
                    return;
                }
                this.CloseSharedChannel("BusConnection.EstablishConnection");
                this.DisposeSharedChannel();
                this.CloseConsumers();
                this.DisposeConnection();
            }

            try
            {
                this._connection = this._connectionFactory.CreateConnection(_config.ConnectionName);
                this._connection.ConnectionRecoveryError += _connection_ConnectionRecoveryError;
                this._connection.CallbackException += _connection_CallbackException;
                this._connection.ConnectionShutdown += _connection_ConnectionShutdown;
                this._connection.RecoverySucceeded += _connection_RecoverySucceeded;
                this.RestoreConsumers();

                this._logger.Verbouse("SDRN.RabbitMQ.EstablishConnection", $"The connection '{this._config.ConnectionName}' is established successfully", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.NotEstablishConnectionToRabbit, "SDRN.RabbitMQ.EstablishConnection", e, this);
                throw new InvalidOperationException($"The connection '{this._config.ConnectionName}'to RabbitMQ is not established", e);
            }
        }

        private void _connection_ConnectionRecoveryError(object sender, ConnectionRecoveryErrorEventArgs e)
        {
            this._logger.Verbouse("SDRN.RabbitMQ.ConnectionRecoveryError", $"The exception is occurred: Connection: '{this._config.ConnectionName}', Message: '{e.Exception.Message}'", this);
            this._logger.Exception(BrokerEvents.ExceptionEvent, "SDRN.RabbitMQ.ConnectionRecoveryError", e.Exception, this);
        }

        private void _connection_CallbackException(object sender, CallbackExceptionEventArgs e)
        {
            this._logger.Verbouse("SDRN.RabbitMQ.CallbackException", $"The exception is occurred: Connection: '{this._config.ConnectionName}', Message: '{e.Exception.Message}'", this);
            this._logger.Exception(BrokerEvents.ExceptionEvent, "SDRN.RabbitMQ.CallbackException", e.Exception, this);
        }

        private void _connection_RecoverySucceeded(object sender, EventArgs e)
        {
            this._logger.Verbouse("SDRN.RabbitMQ.RecoveryConnection", $"The connection '{this._config.ConnectionName}' is recovered successfully", this);
        }

        private void CloseSharedChannel(string reason)
        {
            if (_sharedChannel != null)
            {
                try
                {
                    if (_sharedChannel.IsOpen)
                    {
                        _sharedChannel.Close(200, reason);
                    }
                }
                catch (Exception) { }
            }
        }

        private void DisposeSharedChannel()
        {
            if (_sharedChannel != null)
            {
                try
                {
                    _sharedChannel.Dispose();
                }
                catch (Exception) { }
                _sharedChannel = null;
            }  
        }

        private void _connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            this._logger.Verbouse("SDRN.RabbitMQ.ShutdownConnection", $"The connection '{this._config.ConnectionName}' is shutted down: Initiator: '{e.Initiator}', Reasone: '{e.ReplyText}', Code: #{e.ReplyCode}", this);
        }

        public void EstablishSharedChannel()
        {
            try
            {
                this.EstablishConnection();

                if (_sharedChannel != null)
                {
                    if (_sharedChannel.IsOpen)
                    {
                        return;
                    }
                    this.DisposeSharedChannel();
                }

                this._sharedChannel = this._connection.CreateModel();

                this._logger.Verbouse("SDRN.RabbitMQ.EstablishChannel", $"The shared channel is established successfully", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.NotEstablishRabbitSharedChannel, "SDRN.RabbitMQ.EstablishChannel", e, this);
                throw new InvalidOperationException("The shared channel to RabbitMQ is not established", e);
            }
        }

        private void DisposeConsumers()
        {
            if (_consumers != null)
            {
                foreach (var consumerDescriptor in _consumers)
                {
                    var consumer = consumerDescriptor.Consumer;
                    try
                    {
                        consumer.Dispose();
                    }
                    catch (Exception) { }
                }
                _consumers = null;
            }
        }

        private void CloseConsumers()
        {
            if (_consumers != null)
            {
                foreach (var consumerDescriptor in _consumers)
                {
                    var consumer = consumerDescriptor.Consumer;
                    try
                    {
                        consumer.Dispose();
                    }
                    catch (Exception) { }
                    consumerDescriptor.Consumer = null;
                }
            }
        }

        private void CloseConnection(string reason)
        {
            if (this._connection != null)
            {
                if (this._connection.IsOpen)
                {
                    try
                    {
                        this._connection.Close(200, reason);
                    }
                    catch (Exception) { }
                }
            }
        }

        private void DisposeConnection()
        {
            if (this._connection != null)
            {
                try
                {
                    this._connection.Dispose();
                }
                catch (Exception) { }
                this._connection = null;
            }
        }

        public void Dispose()
        {
            this.CloseSharedChannel("BusConnection.Dispose");
            this.DisposeSharedChannel();
            this.DisposeConsumers();
            this.CloseConnection("BusConnection.Dispose");
            this.DisposeConnection();
        }

        public void DeclareDurableDirectExchange(string exchangeName)
        {
            try
            {
                this.EstablishSharedChannel();

                this._sharedChannel.ExchangeDeclare(
                        exchange: exchangeName,
                        type: "direct",
                        durable: true
                    );

                this._logger.Verbouse("SDRN.RabbitMQ.DeclareExchange", $"The exchange with name '{exchangeName}' is declared successfully: Type = 'Direct', Durable: true", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.NotDeclareExchange, "SDRN.RabbitMQ.DeclareExchange", e, this);
                throw new InvalidOperationException($"The exchange with name '{exchangeName}' is not declared", e);
            }
        }

        public void DeclareDurableQueue(string queue, string exchange, string routingKey)
        {
            try
            {
                this.EstablishSharedChannel();

                _sharedChannel.QueueDeclare(
                           queue: queue,
                           durable: true,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

                _sharedChannel.QueueBind(queue, exchange, routingKey);

                this._logger.Verbouse("SDRN.RabbitMQ.DeclareQueue", $"The queue with name '{queue}' is declared successfully: Routing key = '{routingKey}', Exchange name: '{exchange}'", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.NotDeclareExchange, "RabbitMQ:.DeclareQueue", e, this);
                throw new InvalidOperationException($"The queue with name '{queue}' is not declared", e);
            }
        }

        public void DeclareDurableQueue(string queueName)
        {
            try
            {
                this.EstablishSharedChannel();

                _sharedChannel.QueueDeclare(
                           queue: queueName,
                           durable: true,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

                this._logger.Verbouse("SDRN.RabbitMQ.DeclareQueue", $"The queue with name '{queueName}' is declared successfully", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.NotDeclareExchange, "SDRN.RabbitMQ.DeclareQueue", e, this);
                throw new InvalidOperationException($"The queue with name '{queueName}' is not declared", e);
            }
        }

        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return Convert.ToInt64((TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds);
        }

        public string Publish(string exchange, string routingKey, Message message)
        {
            try
            {
                this.EstablishSharedChannel();

                var props = this._sharedChannel.CreateBasicProperties();
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
                props.Timestamp = new AmqpTimestamp(DateTimeToUnixTimestamp(DateTime.Now));
                props.Headers = message.Headers;

                this._sharedChannel.BasicPublish(exchange, routingKey, props, message.Body);

                this._logger.Verbouse("SDRN.RabbitMQ.PublisheMessage", $"The message '{message.Type}' is published successfully: Exchange: '{exchange}', Routing= '{routingKey}', Id = '{message.Id}'", this);

                return message.Id;
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.NotDeclareExchange, "SDRN.RabbitMQ.PublisheMessage", e, this);
                throw new InvalidOperationException($"The message with type '{message.Type}' is not published", e);
            }
        }

        public void JoinConsumer(string queue, string tag, IMessageHandler handler)
        {
            try
            {
                this.EstablishConnection();

                var channel = this._connection.CreateModel();
                var descriptor = new QueueConsumerDescriptor
                {
                    Queue = queue,
                    Tag = tag,
                    Handler = handler,
                    Consumer = new QueueConsumer(queue, tag, channel, handler)
                };
                this._consumers.Add(descriptor);
                descriptor.Consumer.Join();

                this._logger.Verbouse("SDRN.RabbitMQ.JoinConsumer", $"The consumer '{tag}' is joined successfully: Queue: '{queue}', Handler: '{handler.GetType().FullName}'", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.NotCreateConsumer, "SDRN.RabbitMQ.JoinConsumer", e, this);
                throw new InvalidOperationException($"The consumer '{tag}' is not joined fully", e);
            }
        }

        public void UnjoinConsumer(string queue, string tag, IMessageHandler handler)
        {
            try
            {
                this.EstablishConnection();

                foreach (var descriptor in _consumers)
                {
                    if (descriptor.Queue.Equals(queue, StringComparison.OrdinalIgnoreCase) 
                        && descriptor.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase)
                        && descriptor.Handler == handler)
                    {
                        descriptor.Consumer.Unjoin();
                        break;
                    }

                }

                this._logger.Verbouse("SDRN.RabbitMQ.UnjoinConsumer", $"The consumer '{tag}' is unjoined successfully: Queue: '{queue}', Handler: '{handler.GetType().FullName}'", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BrokerEvents.NotCreateConsumer, "SDRN.RabbitMQ.UnjoinConsumer", e, this);
                throw new InvalidOperationException($"The consumer '{tag}' is not unjoined fully", e);
            }
        }
    }

    
}
