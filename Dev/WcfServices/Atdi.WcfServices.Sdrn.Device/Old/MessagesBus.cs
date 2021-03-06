﻿//using System;
//using Atdi.DataModels.Sdrns.Device;
//using Atdi.Platform.Logging;
//using MMB = Atdi.Modules.Sdrn.MessageBus;
//using System.IO;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;

//namespace Atdi.WcfServices.Sdrn.Device
//{
//    public class MessagesBus : IDisposable
//    {
//        private bool disposedValue = false; 
//        private readonly ILogger _logger;
//        private readonly SdrnServerDescriptor _serverDescriptor;
//        private readonly MMB.MessageConverter _messageConverter;
//        //private ConnectionFactory _connectionFactory;
//        //private IConnection _connection;
//        //private IModel _channel;
//        //private readonly string _exchangeName;
//        private long _fileCounter = 0;
        
//        public MessagesBus(SdrnServerDescriptor serverDescriptor, MMB.MessageConverter messageConverter, ILogger logger)
//        {
//            this._logger = logger;
//            this._serverDescriptor = serverDescriptor;
//            this._messageConverter = messageConverter;
//            //this._connectionFactory= new ConnectionFactory()
//            //{
//            //    HostName = this._serverDescriptor.RabbitMqHost,
//            //    UserName = this._serverDescriptor.RabbitMqUser,
//            //    Password = this._serverDescriptor.RabbitMqPassword
//            //};
//            //if (!string.IsNullOrEmpty(this._serverDescriptor.RabbitMqVirtualHost))
//            //{
//            //    this._connectionFactory.VirtualHost = this._serverDescriptor.RabbitMqVirtualHost;
//            //}

//            //this._exchangeName = $"{this._serverDescriptor.MessagesExchange}.[v{this._serverDescriptor.ApiVersion}]";
//            //this.EstablisheConnection();
//        }

//        public void CheckSensorName(string name)
//        {
//            if (!_serverDescriptor.AllowedSensors.ContainsKey(name))
//            {
//                throw new InvalidOperationException("Attempting to use an unlicensed device");
//            }
//        }

//        //public static long DateTimeToUnixTimestamp(DateTime dateTime)
//        //{
//        //    return  Convert.ToInt64( (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
//        //           new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds);
//        //}

//        public string SaveObject<TObject>(SensorDescriptor descriptor, string messageType, TObject msgObject, string correlationId = null)
//        {
//            if (descriptor == null)
//            {
//                throw new ArgumentNullException(nameof(descriptor));
//            }

//            if (string.IsNullOrEmpty(messageType))
//            {
//                throw new ArgumentException("message", nameof(messageType));
//            }

//            try
//            {
//                var message = this._messageConverter.Pack(messageType, msgObject);
//                message.CorrelationId = correlationId;

//                var messageData = new MessageData
//                {
//                    Created = DateTime.Now,
//                    Descriptor = descriptor,
//                    Message = message
//                };

//                var fileName = MakeFileName();
//                var fullPath = Path.Combine(this._serverDescriptor.MessagesOutboxFolder, fileName);

//                IFormatter formatter = new BinaryFormatter();
//                using (MemoryStream stream = new MemoryStream())
//                {
//                    formatter.Serialize(stream, messageData);
//                    File.WriteAllBytes(fullPath, stream.ToArray());
//                }

//                this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Rabbit MQ", $"The message was saved successfully: MessageType = '{messageType}', SDRN Server = '{descriptor.SdrnServer}', Sensor = '{descriptor.SensorName}', File = '{fileName}'");
//                return message.Id;
//            }
//            catch (Exception e)
//            {
//                this._logger.Exception("SdrnDeviceServices", (EventCategory)"Object saving", e);
//                throw new InvalidOperationException("The object was not saved to outbox", e);
//            }
//        }

//        private string MakeFileName()
//        {
//            var id = System.Threading.Thread.CurrentThread.ManagedThreadId;
//            var d = DateTime.Now;

//            var timeFormat = "yyyyMMdd_HHmmss";
//            var timeString = d.ToString(timeFormat);


//            var timeFormat3 = "FFFFFFF";
//            var timeString3 = d.ToString(timeFormat3).PadRight(timeFormat3.Length, '0');

//            return timeString + "_" + timeString3 + "_" + id.ToString().PadLeft(3, '0') + "_" + (++_fileCounter).ToString().PadLeft(10, '0') + ".mdata";
//        }

//        //public string SendObject<TObject>(SensorDescriptor descriptor, string messageType, TObject msgObject, string correlationId = null)
//        //{
//        //    if (descriptor == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(descriptor));
//        //    }

//        //    if (string.IsNullOrEmpty(messageType))
//        //    {
//        //        throw new ArgumentException("message", nameof(messageType));
//        //    }

//        //    try
//        //    {
//        //        var message = this._messageConverter.Pack(messageType, msgObject);
//        //        message.CorrelationId = correlationId;

//        //        var msgKey = this._serverDescriptor.QueueBindings[messageType].RoutingKey;
//        //        var routingKey = $"[{this._serverDescriptor.ServerInstance}].[{msgKey}]";

//        //        this.EstablisheConnection();

//        //        var props = _channel.CreateBasicProperties();
                    
//        //        props.Persistent = true;
//        //        props.AppId = "Atdi.WcfServices.Sdrn.Device.dll";
//        //        props.MessageId = message.Id;
//        //        props.Type = messageType;
//        //        if (!string.IsNullOrEmpty(message.ContentType))
//        //        {
//        //            props.ContentType = message.ContentType;
//        //        }
//        //        if (!string.IsNullOrEmpty(message.ContentEncoding))
//        //        {
//        //            props.ContentEncoding = message.ContentEncoding;
//        //        }
//        //        if (!string.IsNullOrEmpty(message.CorrelationId))
//        //        {
//        //            props.CorrelationId = message.CorrelationId;
//        //        }
//        //        props.Timestamp = new AmqpTimestamp(DateTimeToUnixTimestamp(DateTime.Now));
 
//        //        props.Headers = new Dictionary<string, object>
//        //        {
//        //            ["SdrnServer"] = descriptor.SdrnServer,
//        //            ["SensorName"] = descriptor.SensorName,
//        //            ["SensorTechId"] = descriptor.EquipmentTechId,
//        //            ["Created"] = DateTime.Now.ToString("o")
//        //        };

//        //        _channel.BasicPublish(exchange: this._exchangeName,
//        //                            routingKey: routingKey,
//        //                            basicProperties: props,
//        //                            body: message.Body);

//        //        this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Rabbit MQ", $"The message was sent successfully: MessageType = '{messageType}', RoutingKey = '{routingKey}', SDRN Server = '{descriptor.SdrnServer}', Sensor = '{descriptor.SensorName}'");
//        //        return message.Id;
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        this._logger.Exception("SdrnDeviceServices", (EventCategory)"Object sending", e);
//        //        throw new InvalidOperationException("The object was not sent to server", e);
//        //    }
//        //}

//        //public TObject TryGetObject<TObject>(SensorDescriptor descriptor, string messageType, out string token)
//        //{
//        //    if (descriptor == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(descriptor));
//        //    }
//        //    if (string.IsNullOrEmpty(messageType))
//        //    {
//        //        throw new ArgumentException("Invalid argument", nameof(messageType));
//        //    }

//        //    try
//        //    {
//        //        token = string.Empty;

//        //        this.EstablisheConnection();

//        //        var deviceQueueName = this.DeclareDeviceQueue(descriptor, _channel);
//        //        var message = default(BasicGetResult);

//        //        using (var tryChannel = this._connection.CreateModel())
//        //        {
//        //            do
//        //            {
//        //                message = tryChannel.BasicGet(deviceQueueName, false);
//        //                if (message == null)
//        //                {
//        //                    tryChannel.Close();
//        //                    return default(TObject);
//        //                }

//        //                if (messageType.Equals(message.BasicProperties.Type, StringComparison.OrdinalIgnoreCase))
//        //                {
//        //                    token = message.BasicProperties.MessageId;
//        //                    tryChannel.Close();

//        //                    var msg = new MMB.Message
//        //                    {
//        //                        Id = message.BasicProperties.MessageId,
//        //                        Type = message.BasicProperties.Type,
//        //                        ContentType = message.BasicProperties.ContentType,
//        //                        ContentEncoding = message.BasicProperties.ContentEncoding,
//        //                        Body = message.Body
//        //                    };

//        //                    var messageObject = this._messageConverter.Deserialize(msg);
//        //                    return (TObject) messageObject.Object;
//        //                }
//        //            }
//        //            while (message.MessageCount > 0);

//        //            tryChannel.Close();
//        //        }
                    

//        //        return default(TObject);
//        //    }
//        //    catch(Exception e)
//        //    {
//        //        this._logger.Exception("SdrnDeviceServices", (EventCategory)"Object getting", e);
//        //        throw new InvalidOperationException("The object was not gotten from server", e);
//        //    }
//        //}

//        //public TObject WaitObject<TObject>(SensorDescriptor descriptor, string messageType, string correlationId)
//        //{
//        //    if (descriptor == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(descriptor));
//        //    }
//        //    if (string.IsNullOrEmpty(messageType))
//        //    {
//        //        throw new ArgumentException("message", nameof(messageType));
//        //    }

//        //    try
//        //    {
//        //        var result = default(TObject);

//        //        this.EstablisheConnection();

//        //        var deviceQueueName = this.DeclareDeviceQueue(descriptor, _channel);

//        //        var respQueue = new BlockingCollection<MMB.Message>();

//        //        var consumer = new EventingBasicConsumer(_channel);
//        //        consumer.Received += (model, ea) =>
//        //        {
//        //            var respMessageType = ea.BasicProperties.Type;
//        //            if (string.IsNullOrEmpty(respMessageType))
//        //            {
//        //                _channel.BasicReject(ea.DeliveryTag, false);
//        //                return;
//        //            }
//        //            if (!respMessageType.Equals(messageType, StringComparison.OrdinalIgnoreCase))
//        //            {
//        //                _channel.BasicNack(ea.DeliveryTag, false, true);
//        //                return;
//        //            }
//        //            if (!string.IsNullOrEmpty(correlationId))
//        //            {
//        //                if (string.IsNullOrEmpty(ea.BasicProperties.CorrelationId))
//        //                {
//        //                    _channel.BasicNack(ea.DeliveryTag, false, true);
//        //                    return;
//        //                }

//        //                if (ea.BasicProperties.CorrelationId != correlationId)
//        //                {
//        //                    _channel.BasicNack(ea.DeliveryTag, false, true);
//        //                    return;
//        //                }
//        //            }

//        //            var sdrnMsg = new MMB.Message
//        //            {
//        //                Id = ea.BasicProperties.MessageId,
//        //                Type = ea.BasicProperties.Type,
//        //                ContentType = ea.BasicProperties.ContentType,
//        //                ContentEncoding = ea.BasicProperties.ContentEncoding,
//        //                Body = ea.Body
//        //            };
//        //            _channel.BasicAck(ea.DeliveryTag, false);
//        //            respQueue.Add(sdrnMsg);
//        //        };

//        //        var consumerTag = $"RPC: {Guid.NewGuid().ToString()}";
//        //        _channel.BasicConsume(
//        //            consumer: consumer,
//        //            queue: deviceQueueName,
//        //            consumerTag: consumerTag,
//        //            autoAck: false);

//        //        try
//        //        {
//        //            var sdrnMsg = respQueue.Take();
//        //            var msgObject = this._messageConverter.Deserialize(sdrnMsg);
//        //            result = (TObject)msgObject.Object;
//        //        }
//        //        catch(Exception)
//        //        {
//        //            throw;
//        //        }
//        //        finally
//        //        {
//        //            _channel.BasicCancel(consumerTag);
//        //        }

//        //        return result;
//        //    }
//        //    catch(Exception e)
//        //    {
//        //        this._logger.Exception("SdrnDeviceServices", (EventCategory)"Object waitting", e);
//        //        throw new InvalidOperationException("The object was not waited from server", e);
//        //    }
//        //}

//        //public void AckMessage(SensorDescriptor descriptor, byte[] token)
//        //{
//        //    if (descriptor == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(descriptor));
//        //    }

//        //    if (token == null)
//        //    {
//        //        throw new ArgumentNullException(nameof(token));
//        //    }
//        //    if (token.Length == 0)
//        //    {
//        //        throw new ArgumentException("Invalid token");
//        //    }

//        //    try
//        //    {
//        //        var messageId = Encoding.UTF8.GetString(token);

//        //        this.EstablisheConnection();

//        //        var deviceQueueName = this.DeclareDeviceQueue(descriptor, _channel);
//        //        var message = default(BasicGetResult);

//        //        using (var tryChannel = this._connection.CreateModel())
//        //        {
//        //            do
//        //            {
//        //                message = tryChannel.BasicGet(deviceQueueName, false);
//        //                if (message == null)
//        //                {
//        //                    tryChannel.Close();
//        //                    return;
//        //                }

//        //                var respMessageId = message.BasicProperties.MessageId;
//        //                if (!string.IsNullOrEmpty(respMessageId))
//        //                {
//        //                    if (respMessageId.Equals(messageId, StringComparison.OrdinalIgnoreCase))
//        //                    {
//        //                        tryChannel.BasicAck(message.DeliveryTag, false);
//        //                        tryChannel.Close();
//        //                        this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Rabbit MQ", $"The message was ACK successfully: MessageId = '{messageId}', Type = '{message.BasicProperties.Type}', RoutingKey = '{message.RoutingKey}', SDRN Server = '{descriptor.SdrnServer}', Sensor = '{descriptor.SensorName}'");
//        //                        return;
//        //                    }
//        //                }
//        //            }
//        //            while (message.MessageCount > 0);
//        //            tryChannel.Close();
//        //        }
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        this._logger.Exception("SdrnDeviceServices", (EventCategory)"Message is ack", e);
//        //        throw new InvalidOperationException("The message was not ack", e);
//        //    }
//        //}

//        //private void EstablisheConnection()
//        //{
//        //    var connectionName = $"SDRN Device [{this._serverDescriptor.Instance}] (MessagesBus) #{System.Threading.Thread.CurrentThread.ManagedThreadId}";
//        //    if (this._connection == null)
//        //    {
//        //        this._connection = this._connectionFactory.CreateConnection(connectionName);
//        //    }
//        //    else if (!this._connection.IsOpen)
//        //    {
//        //        this._connection.Dispose();
//        //        this._connection = this._connectionFactory.CreateConnection(connectionName);
//        //    }
//        //    if (this._channel == null)
//        //    {
//        //        this._channel = this._connection.CreateModel();
//        //    }
//        //    else if (this._channel.IsClosed)
//        //    {
//        //        this._channel.Dispose();
//        //        this._channel = this._connection.CreateModel();
//        //    }
//        //}
//        //private string GetHeaderValue(string key, IDictionary<string, object> headers)
//        //{
//        //    if (!headers.ContainsKey(key))
//        //    {
//        //        return null;
//        //    }
//        //    return Convert.ToString(Encoding.UTF8.GetString(((byte[])headers[key])));
//        //}
//        //private string DeclareDeviceQueue(SensorDescriptor descriptor, IModel channel)
//        //{
//        //    var deviceQueueName = $"{this._serverDescriptor.DeviceQueueNamePart}.[{descriptor.SensorName}].[{descriptor.EquipmentTechId}].[v{this._serverDescriptor.ApiVersion}]";

//        //    channel.QueueDeclare(
//        //        queue: deviceQueueName,
//        //        durable: true,
//        //        exclusive: false,
//        //        autoDelete: false,
//        //        arguments: null);

//        //    return deviceQueueName;
//        //}

//        //private byte[] SerializeObjectToByteArray<TObject>(TObject source)
//        //{
//        //    if (source == null)
//        //    {
//        //        return new byte[] { };
//        //    }

//        //    byte[] result = null;

//        //    var json = JsonConvert.SerializeObject(source);
//        //    result = Encoding.UTF8.GetBytes(json);

//        //    return result;
//        //}

//        //private TObject DeserializeObjectFromByteArray<TObject>(byte[] source)
//        //{
//        //    if (source == null)
//        //    {
//        //        return default(TObject);
//        //    }

//        //    TObject result = default(TObject);

//        //    var json = Encoding.UTF8.GetString(source);
//        //    result = JsonConvert.DeserializeObject<TObject>(json);

//        //    return result;
//        //}

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!disposedValue)
//            {
//                if (disposing)
//                {
//                    //if (this._channel != null)
//                    //{
//                    //    if (this._channel.IsOpen)
//                    //    {
//                    //        this._channel.Close();
//                    //    }
//                    //    this._channel.Dispose();
//                    //    this._channel = null;
//                    //}
//                    //if (this._connection != null)
//                    //{
//                    //    if (this._connection.IsOpen)
//                    //    {
//                    //        this._connection.Close();
//                    //    }
//                    //    this._connection.Dispose();
//                    //    this._connection = null;
//                    //}
//                }
//                disposedValue = true;
//            }
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//        }

//    }
//}
