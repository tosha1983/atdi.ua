using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;
using RabbitMQ.Client;
using Newtonsoft;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using RabbitMQ.Client.Events;

namespace Atdi.WcfServices.Sdrn.Device
{
    public class MessagesBus : IDisposable
    {
        private bool disposedValue = false; 
        private readonly ILogger _logger;
        private readonly SdrnServerDescriptor _serverDescriptor;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private readonly string _exchangeName;

        
        public MessagesBus(SdrnServerDescriptor serverDescriptor, ILogger logger)
        {
            this._logger = logger;
            this._serverDescriptor = serverDescriptor;
            this._connectionFactory= new ConnectionFactory()
            {
                HostName = this._serverDescriptor.RabbitMqHost,
                UserName = this._serverDescriptor.RabbitMqUser,
                Password = this._serverDescriptor.RabbitMqPassword
            };

            this._connection = this._connectionFactory.CreateConnection();

            this._exchangeName = $"{this._serverDescriptor.MessagesExchange}.[v{this._serverDescriptor.ApiVersion}]";
        }

        public string SendObject<TObject>(SensorDescriptor descriptor, string messageType, TObject msgObject, string correlationId = null)
        {
            try
            {
                var data = this.SerializeObjectToByteArray(msgObject);

                var msgKey = this._serverDescriptor.QueueBindings[messageType].RoutingKey;

                var routingKey = $"[{this._serverDescriptor.ServerInstance}].[{msgKey}]";


                if (!this._connection.IsOpen)
                {
                    this._connection.Dispose();
                    this._connection = this._connectionFactory.CreateConnection();
                }

                using (var channel = this._connection.CreateModel())
                {
                    var messageId = Guid.NewGuid().ToString();

                    var props = channel.CreateBasicProperties();
                    
                    props.Persistent = true;
                    props.AppId = "Atdi.WcfServices.Sdrn.Device.dll";
                    props.MessageId = messageId;
                    props.Type = messageType;

                    if (!string.IsNullOrEmpty(correlationId))
                    {
                        props.CorrelationId = correlationId;
                    }

                    props.Headers = new Dictionary<string, object>();
                    props.Headers["SdrnServer"] = descriptor.SdrnServer;
                    props.Headers["SensorName"] = descriptor.SensorName;
                    props.Headers["SensorTechId"] = descriptor.EquipmentTechId;

                    channel.BasicPublish(exchange: this._exchangeName,
                                     routingKey: routingKey,
                                     basicProperties: props,
                                     body: data);

                    this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Rabbit MQ", $"The message was sent successfuly: MessageType = '{messageType}', RoutingKey = '{routingKey}', SDRN Server = '{descriptor.SdrnServer}', Sensor = '{descriptor.SensorName}'");

                    channel.Close();

                    return messageId;
                }

            }
            catch (Exception e)
            {
                this._logger.Exception("SdrnDeviceServices", (EventCategory)"Object sending", e);
                throw new InvalidOperationException("The object was not sent to server", e);
            }
        }

        public TObject TryGetObject<TObject>(SensorDescriptor descriptor, string messageType)
        {
            var result = default(TObject);
            return result;
        }

        public TObject WaiteObject<TObject>(SensorDescriptor descriptor, string messageType, string correlationId)
        {
            var result = default(TObject);
            
            if (!this._connection.IsOpen)
            {
                this._connection.Dispose();
                this._connection = this._connectionFactory.CreateConnection();
            }

            using (var channel = this._connection.CreateModel())
            {

                var deviceQueueName = this.DeclareDeviceQueue(descriptor, channel);

                var respQueue = new BlockingCollection<byte[]>();

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    if (string.IsNullOrEmpty(ea.BasicProperties.CorrelationId) && !string.IsNullOrEmpty(correlationId))
                    {
                        channel.BasicAck(ea.DeliveryTag, false);
                        return;
                    }

                    var respMessageType = ea.BasicProperties.Type;
                    if (string.IsNullOrEmpty(respMessageType))
                    {
                        channel.BasicAck(ea.DeliveryTag, false);
                        return;
                    }

                    if (!respMessageType.Equals(messageType, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }

                    if (!string.IsNullOrEmpty(correlationId))
                    {
                        if (ea.BasicProperties.CorrelationId != correlationId)
                        {
                            return;
                        }
                    }
                    

                    channel.BasicAck(ea.DeliveryTag, false);
                    respQueue.Add(ea.Body);
                };

                channel.BasicConsume(
                    consumer: consumer,
                    queue: deviceQueueName,
                    autoAck: false );

                result = this.DeserializeObjectFromByteArray<TObject>(respQueue.Take());

                channel.Close();
            }

            return result;
        }

        private string GetHeaderValue(string key, IDictionary<string, object> headers)
        {
            if (!headers.ContainsKey(key))
            {
                return null;
            }
            return Convert.ToString(Encoding.UTF8.GetString(((byte[])headers[key])));
        }
        private string DeclareDeviceQueue(SensorDescriptor descriptor, IModel channel)
        {
            var deviceQueueName = $"{this._serverDescriptor.DeviceQueueNamePart}.[{descriptor.SensorName}].[{descriptor.EquipmentTechId}].[v{this._serverDescriptor.ApiVersion}]";

            channel.QueueDeclare(
                queue: deviceQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            return deviceQueueName;
        }

        private byte[] SerializeObjectToByteArray<TObject>(TObject source)
        {
            byte[] result = null;

            var json = JsonConvert.SerializeObject(source);

            result = Encoding.UTF8.GetBytes(json);

            return result;
        }

        private TObject DeserializeObjectFromByteArray<TObject>(byte[] source)
        {
            if (source == null)
            {
                return default(TObject);
            }

            TObject result = default(TObject);

            var json = Encoding.UTF8.GetString(source);
            result = JsonConvert.DeserializeObject<TObject>(json);


            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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
                disposedValue = true;
            }
        }


        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

    }
}
