using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Atdi.Platform.DependencyInjection;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class RabbitMQConnection : LoggedObject, IDisposable
    {
        private readonly string _siteName;
        private readonly SdrnServerDescriptor _serverDescriptor;
        private readonly IServicesResolver _servicesResolver;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _sharedChannel;
        private readonly EventCategory _eventCategory;
        private readonly EventContext _eventContext;
        private List<QueueConsumer> _consumers;
        private List<IModel> _channels;

        public RabbitMQConnection(string siteName, SdrnServerDescriptor serverDescriptor, IServicesResolver servicesResolver, ILogger logger) : base(logger)
        {
            this._siteName = siteName;
            this._serverDescriptor = serverDescriptor;
            this._servicesResolver = servicesResolver;
            this._eventCategory = (EventCategory)$"Rabbit MQ: {this._serverDescriptor.RabbitMqHost}";
            this._eventContext = $"SDRN.Server.[{_serverDescriptor.ServerInstance}].{this._siteName}";
            this._consumers = new List<QueueConsumer>();
            this._channels = new List<IModel>();
            this._connectionFactory = new ConnectionFactory()
            {
                HostName = this._serverDescriptor.RabbitMqHost,
                UserName = this._serverDescriptor.RabbitMqUser,
                Password = this._serverDescriptor.RabbitMqPassword,

            };

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
                this._connection = this._connectionFactory.CreateConnection($"SDRN.Server.[{_serverDescriptor.ServerInstance}].{this._siteName}");

                this.Logger.Verbouse(this._eventContext, this._eventCategory, $"The connection to Rabbit MQ Server is established successfully");
            }
            catch (Exception e)
            {
                this.Logger.Exception(this._eventContext, this._eventCategory, e);
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

                this.Logger.Verbouse(this._eventContext, this._eventCategory, $"The shared channel is established successfully");
            }
            catch (Exception e)
            {
                this.Logger.Exception(this._eventContext, this._eventCategory, e);
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

            if (_consumers != null)
            {
                _consumers = null;
            }

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

                this.Logger.Verbouse(this._eventContext, this._eventCategory, $"The exchange with name '{exchangeName}' is declared successfully: Type = 'Direct', Durable: true");
            }
            catch (Exception e)
            {
                this.Logger.Exception(this._eventContext, this._eventCategory, e);
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

                this.Logger.Verbouse(this._eventContext, this._eventCategory, $"The queue with name '{queueDescriptor.Name}' is declared successfully: Routing key = '{queueDescriptor.RoutingKey}', Exchange name: '{queueDescriptor.Exchange}'");
            }
            catch (Exception e)
            {
                this.Logger.Exception(this._eventContext, this._eventCategory, e);
                throw new InvalidOperationException($"The queue with name '{queueDescriptor.Name}' is not declared", e);
            }
        }

        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return Convert.ToInt64((TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds);
        }

        public string Publish(string exchange, string routingKey, IDictionary<string, object> headers, string type, byte[] body, string correlationId = null)
        {
            try
            {
                this.EstablishSharedChannel();

                var messageId = Guid.NewGuid().ToString();

                var props = this._sharedChannel.CreateBasicProperties();
                props.Persistent = true;
                props.AppId = "Atdi.AppUnits.Sdrn.BusController.dll";
                props.MessageId = messageId;
                props.Type = type;
                props.Timestamp = new AmqpTimestamp(DateTimeToUnixTimestamp(DateTime.Now));
                if (!string.IsNullOrEmpty(correlationId))
                {
                    props.CorrelationId = correlationId;
                }

                props.Headers = headers;

                this._sharedChannel.BasicPublish(exchange, routingKey, props, body);

                this.Logger.Verbouse(this._eventContext, this._eventCategory, $"The message '{messageId}' is published successfully: Routing key = '{routingKey}', Exchange name: '{exchange}'");

                return messageId;
            }
            catch (Exception e)
            {
                this.Logger.Exception(this._eventContext, this._eventCategory, e);
                throw new InvalidOperationException($"The message with type '{type}' is not published", e);
            }
        }

        public QueueConsumer[] CreateConsumers(string consumerNamePart, string queueName, int count)
        {
            try
            {
                var channel = this._connection.CreateModel();
                this._channels.Add(channel);
                var consumers = new QueueConsumer[count];
                for (int i = 0; i < count; i++)
                {
                    var consumerName = $"[{consumerNamePart}].[#{i}]";
                    var consumer = new QueueConsumer(consumerName, queueName, this._serverDescriptor, channel, this._servicesResolver, this.Logger);
                    this._consumers.Add(consumer);
                    consumers[i] = consumer;
                }

                return consumers;
            }
            catch (Exception e)
            {
                this.Logger.Exception(this._eventContext, this._eventCategory, e);
                throw new InvalidOperationException($"The consumers '{consumerNamePart}' is not created fully", e);
            }
        }
    }

    public class PublisherRabbitMQConnection : RabbitMQConnection
    {
        public PublisherRabbitMQConnection(SdrnServerDescriptor serverDescriptor, IServicesResolver servicesResolver, ILogger logger) 
            : base("Publisher", serverDescriptor, servicesResolver, logger)
        {
        }
    }

    public class ConsumersRabbitMQConnection : RabbitMQConnection
    {
        public ConsumersRabbitMQConnection(SdrnServerDescriptor serverDescriptor, IServicesResolver servicesResolver, ILogger logger)
            : base("Consumers", serverDescriptor, servicesResolver, logger)
        {
        }

        
    }
}
