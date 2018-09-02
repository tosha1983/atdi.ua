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
        private IModel _channel;
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
            this._exchangeName = $"{this._serverDescriptor.MessagesExchange}.[v{this._serverDescriptor.ApiVersion}]";
            this.EstablisheConnection();
        }

        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return  Convert.ToInt64( (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds);
        }

        public string SendObject<TObject>(SensorDescriptor descriptor, string messageType, TObject msgObject, string correlationId = null)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (string.IsNullOrEmpty(messageType))
            {
                throw new ArgumentException("message", nameof(messageType));
            }

            try
            {
                var data = this.SerializeObjectToByteArray(msgObject);

                var msgKey = this._serverDescriptor.QueueBindings[messageType].RoutingKey;

                var routingKey = $"[{this._serverDescriptor.ServerInstance}].[{msgKey}]";

                this.EstablisheConnection();

                var messageId = Guid.NewGuid().ToString();

                var props = _channel.CreateBasicProperties();
                    
                props.Persistent = true;
                props.AppId = "Atdi.WcfServices.Sdrn.Device.dll";
                props.MessageId = messageId;
                props.Type = messageType;
                props.Timestamp = new AmqpTimestamp(DateTimeToUnixTimestamp(DateTime.Now));
 
                if (!string.IsNullOrEmpty(correlationId))
                {
                    props.CorrelationId = correlationId;
                }

                props.Headers = new Dictionary<string, object>
                {
                    ["SdrnServer"] = descriptor.SdrnServer,
                    ["SensorName"] = descriptor.SensorName,
                    ["SensorTechId"] = descriptor.EquipmentTechId,
                    ["Created"] = DateTime.Now.ToString("o")
                };

                _channel.BasicPublish(exchange: this._exchangeName,
                                    routingKey: routingKey,
                                    basicProperties: props,
                                    body: data);

                this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Rabbit MQ", $"The message was sent successfully: MessageType = '{messageType}', RoutingKey = '{routingKey}', SDRN Server = '{descriptor.SdrnServer}', Sensor = '{descriptor.SensorName}'");
                return messageId;
            }
            catch (Exception e)
            {
                this._logger.Exception("SdrnDeviceServices", (EventCategory)"Object sending", e);
                throw new InvalidOperationException("The object was not sent to server", e);
            }
        }

        public TObject TryGetObject<TObject>(SensorDescriptor descriptor, string messageType, out string token)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }
            if (string.IsNullOrEmpty(messageType))
            {
                throw new ArgumentException("Invalid argument", nameof(messageType));
            }

            try
            {
                token = string.Empty;

                this.EstablisheConnection();

                var deviceQueueName = this.DeclareDeviceQueue(descriptor, _channel);
                var message = default(BasicGetResult);

                using (var tryChannel = this._connection.CreateModel())
                {
                    do
                    {
                        message = tryChannel.BasicGet(deviceQueueName, false);
                        if (message == null)
                        {
                            tryChannel.Close();
                            return default(TObject);
                        }

                        if (messageType.Equals(message.BasicProperties.Type, StringComparison.OrdinalIgnoreCase))
                        {
                            token = message.BasicProperties.MessageId;
                            tryChannel.Close();
                            return this.DeserializeObjectFromByteArray<TObject>(message.Body);
                        }
                    }
                    while (message.MessageCount > 0);

                    tryChannel.Close();
                }
                    

                return default(TObject);
            }
            catch(Exception e)
            {
                this._logger.Exception("SdrnDeviceServices", (EventCategory)"Object getting", e);
                throw new InvalidOperationException("The object was not gotten from server", e);
            }
        }

        public TObject WaitObject<TObject>(SensorDescriptor descriptor, string messageType, string correlationId)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }
            if (string.IsNullOrEmpty(messageType))
            {
                throw new ArgumentException("message", nameof(messageType));
            }

            try
            {
                var result = default(TObject);

                this.EstablisheConnection();

                var deviceQueueName = this.DeclareDeviceQueue(descriptor, _channel);

                var respQueue = new BlockingCollection<byte[]>();

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    var respMessageType = ea.BasicProperties.Type;
                    if (string.IsNullOrEmpty(respMessageType))
                    {
                        _channel.BasicReject(ea.DeliveryTag, false);
                        return;
                    }
                    if (!respMessageType.Equals(messageType, StringComparison.OrdinalIgnoreCase))
                    {
                        _channel.BasicNack(ea.DeliveryTag, false, true);
                        return;
                    }
                    if (!string.IsNullOrEmpty(correlationId))
                    {
                        if (string.IsNullOrEmpty(ea.BasicProperties.CorrelationId))
                        {
                            _channel.BasicNack(ea.DeliveryTag, false, true);
                            return;
                        }

                        if (ea.BasicProperties.CorrelationId != correlationId)
                        {
                            _channel.BasicNack(ea.DeliveryTag, false, true);
                            return;
                        }
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                    respQueue.Add(ea.Body);
                };

                var consumerTag = $"RPC: {Guid.NewGuid().ToString()}";
                _channel.BasicConsume(
                    consumer: consumer,
                    queue: deviceQueueName,
                    consumerTag: consumerTag,
                    autoAck: false);

                try
                {
                    var body = respQueue.Take();
                    result = this.DeserializeObjectFromByteArray<TObject>(body);
                }
                catch(Exception)
                {
                    throw;
                }
                finally
                {
                    _channel.BasicCancel(consumerTag);
                }

                return result;
            }
            catch(Exception e)
            {
                this._logger.Exception("SdrnDeviceServices", (EventCategory)"Object waitting", e);
                throw new InvalidOperationException("The object was not waited from server", e);
            }
        }

        public void AckMessage(SensorDescriptor descriptor, byte[] token)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            if (token.Length == 0)
            {
                throw new ArgumentException("Invalid token");
            }

            try
            {
                var messageId = Encoding.UTF8.GetString(token);

                this.EstablisheConnection();

                var deviceQueueName = this.DeclareDeviceQueue(descriptor, _channel);
                var message = default(BasicGetResult);

                using (var tryChannel = this._connection.CreateModel())
                {
                    do
                    {
                        message = tryChannel.BasicGet(deviceQueueName, false);
                        if (message == null)
                        {
                            tryChannel.Close();
                            return;
                        }

                        var respMessageId = message.BasicProperties.MessageId;
                        if (!string.IsNullOrEmpty(respMessageId))
                        {
                            if (respMessageId.Equals(messageId, StringComparison.OrdinalIgnoreCase))
                            {
                                tryChannel.BasicAck(message.DeliveryTag, false);
                                tryChannel.Close();
                                this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Rabbit MQ", $"The message was ACK successfully: MessageId = '{messageId}', Type = '{message.BasicProperties.Type}', RoutingKey = '{message.RoutingKey}', SDRN Server = '{descriptor.SdrnServer}', Sensor = '{descriptor.SensorName}'");
                                return;
                            }
                        }
                    }
                    while (message.MessageCount > 0);
                    tryChannel.Close();
                }
            }
            catch (Exception e)
            {
                this._logger.Exception("SdrnDeviceServices", (EventCategory)"Message is ack", e);
                throw new InvalidOperationException("The message was not ack", e);
            }
        }

        private void EstablisheConnection()
        {
            var connectionName = $"SDRN Device [{this._serverDescriptor.Instance}] (MessagesBus) #{System.Threading.Thread.CurrentThread.ManagedThreadId}";
            if (this._connection == null)
            {
                this._connection = this._connectionFactory.CreateConnection(connectionName);
            }
            else if (!this._connection.IsOpen)
            {
                this._connection.Dispose();
                this._connection = this._connectionFactory.CreateConnection(connectionName);
            }
            if (this._channel == null)
            {
                this._channel = this._connection.CreateModel();
            }
            else if (this._channel.IsClosed)
            {
                this._channel.Dispose();
                this._channel = this._connection.CreateModel();
            }
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
            if (source == null)
            {
                return new byte[] { };
            }

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
                    if (this._channel != null)
                    {
                        if (this._channel.IsOpen)
                        {
                            this._channel.Close();
                        }
                        this._channel.Dispose();
                        this._channel = null;
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
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

    }
}
