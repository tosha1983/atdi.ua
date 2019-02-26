using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Atdi.Modules.Sdrn.MessageBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal class RabbitMQBus : IDisposable
    {
        private readonly string _contextName;
        private readonly EnvironmentDescriptor _environmentDescriptor;
        private readonly BusLogger _logger;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _sharedChannel;
        //private List<QueueConsumer> _consumers;
        private List<IModel> _channels;

        public RabbitMQBus(string contextName, EnvironmentDescriptor environmentDescriptor, BusLogger logger)
        {
            this._contextName = contextName;
            this._environmentDescriptor = environmentDescriptor;
            this._logger = logger;

            //this._consumers = new List<QueueConsumer>();
            this._channels = new List<IModel>();
            this._connectionFactory = new ConnectionFactory()
            {
                HostName = this._environmentDescriptor.RabbitMQHost,
                UserName = this._environmentDescriptor.RabbitMQUser,
                Password = this._environmentDescriptor.RabbitMQPassword,
            };

            if (!string.IsNullOrEmpty(this._environmentDescriptor.RabbitMQPort))
            {
                this._connectionFactory.Port = int.Parse(this._environmentDescriptor.RabbitMQPort);
            }

            if (!string.IsNullOrEmpty(this._environmentDescriptor.RabbitMQVirtualHost))
            {
                this._connectionFactory.VirtualHost = this._environmentDescriptor.RabbitMQVirtualHost;
            }

            this.EstablishConnection();
        }

        public void EstablishConnection()
        {
            if (this._connection != null)
            {
                if (this._connection.IsOpen)
                {
                    return;
                }
                if (_sharedChannel != null)
                {
                    if (_sharedChannel.IsOpen)
                    {
                        _sharedChannel.Close();
                    }
                    _sharedChannel.Dispose();
                    _sharedChannel = null;
                }
                
                this._connection.Dispose();
                this._connection = null;
            }

            try
            {
                this._connection = this._connectionFactory.CreateConnection($"SDRN.Device.[{_environmentDescriptor.SdrnDeviceSensorName}].{this._contextName}");
                this._logger.Verbouse("Rabbit MQ: Establish Connection", $"The connection to Rabbit MQ Server is established successfully", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BusEvents.NotEstablishConnectionToRabbit, "Rabbit MQ: Establish Connection", e, this);
                throw new InvalidOperationException("The connection to RabbitMQ is not established", e);
            }
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
                    _sharedChannel.Dispose();
                    _sharedChannel = null;
                }

                this._sharedChannel = this._connection.CreateModel();

                this._logger.Verbouse("Rabbit MQ: Establish Channel", $"The shared channel is established successfully", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BusEvents.NotEstablishRabbitSharedChannel, "Rabbit MQ: Establish Channel", e, this);
                throw new InvalidOperationException("The shared channel to RabbitMQ is not established", e);
            }
        }

        public void Dispose()
        {
            if (_sharedChannel != null)
            {
                if (_sharedChannel.IsOpen)
                {
                    _sharedChannel.Close();
                }
                _sharedChannel.Dispose();
                _sharedChannel = null;
            }

            //if (_consumers != null)
            //{
            //    _consumers = null;
            //}

            if (_channels != null)
            {
                foreach (var channel in _channels)
                {
                    if (channel.IsOpen)
                    {
                        channel.Close();
                    }
                    channel.Dispose();
                }
                _channels = null;
            }

            if (this._connection != null)
            {
                if (this._connection.IsOpen)
                {
                    this._connection.Close();
                }
                this._connection.Dispose();
                this._connection = null;
            }
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

                this._logger.Verbouse("Rabbit MQ: Declare Exchange", $"The exchange with name '{exchangeName}' is declared successfully: Type = 'Direct', Durable: true", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BusEvents.NotDeclareExchange, "Rabbit MQ: Declare Exchange", e, this);
                throw new InvalidOperationException($"The exchange with name '{exchangeName}' is not declared", e);
            }
        }

        public void DeclareDurableQueue(QueueDescriptor queueDescriptor)
        {
            try
            {
                this.EstablishSharedChannel();

                _sharedChannel.QueueDeclare(
                           queue: queueDescriptor.Name,
                           durable: true,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

                _sharedChannel.QueueBind(queueDescriptor.Name, queueDescriptor.Exchange, queueDescriptor.RoutingKey);

                this._logger.Verbouse("Rabbit MQ: Declare Queue", $"The queue with name '{queueDescriptor.Name}' is declared successfully: Routing key = '{queueDescriptor.RoutingKey}', Exchange name: '{queueDescriptor.Exchange}'", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BusEvents.NotDeclareExchange, "Rabbit MQ: Declare Queue", e, this);
                throw new InvalidOperationException($"The queue with name '{queueDescriptor.Name}' is not declared", e);
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

                

                this._logger.Verbouse("Rabbit MQ: Declare Queue", $"The queue with name '{queueName}' is declared successfully", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BusEvents.NotDeclareExchange, "Rabbit MQ: Declare Queue", e, this);
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
                props.AppId = "Atdi.Api.Sdrn.Device.BusController.dll";
                props.MessageId = message.Id;
                props.Type = message.Type;
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

                this._logger.Verbouse("Rabbit MQ: Publishe Message", $"The message '{message.Type}' is published successfully: Id = '{message.Id}', Routing key = '{routingKey}', Exchange name: '{exchange}'", this);

                return message.Id;
            }
            catch (Exception e)
            {
                this._logger.Exception(BusEvents.NotDeclareExchange, "Rabbit MQ: Publishe Message", e, this);
                throw new InvalidOperationException($"The message with type '{message.Type}' is not published", e);
            }
        }

        public QueueConsumer CreateConsumer(string queueName, string consumerName, MessageDispatcher messageDispatcher)
        {
            try
            {
                this.EstablishConnection();

                var channel = this._connection.CreateModel();
                channel.BasicQos(0, 1, false);
                this._channels.Add(channel);

                var consumer = new QueueConsumer(consumerName, queueName, _environmentDescriptor, messageDispatcher, this, channel, this._logger);

                return consumer;
            }
            catch (Exception e)
            {
                this._logger.Exception(BusEvents.NotCreateConsumer, "Rabbit MQ: Create Consumer", e, this);
                throw new InvalidOperationException($"The consumer '{consumerName}' is not created fully", e);
            }
        }
    }

    
}
