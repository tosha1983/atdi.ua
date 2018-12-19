using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using RabbitMQ.Client;
using MMB = Atdi.Modules.Sdrn.MessageBus;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace Atdi.WcfServices.Sdrn.Device
{
    class BusPublisher : IDisposable
    {
        private enum MessageSendingState
        {
            None = 0,
            Sending,
            Returned,
            Sent,
            Nacks,
            Aborted
        }

        private readonly SdrnServerDescriptor _serverDescriptor;
        private readonly MMB.MessageConverter _messageConverter;
        private readonly ILogger _logger;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _publishChannel;
        private readonly string _exchangeName;
        private Task _mainTask;

        private int _sendingTimeout = 1000 * 5;
        private int _startingTimeout = 1000 * 5;
        private int _declaringTimeout = 1000 * 10;
        private int _connectionTimeout = 1000 * 10;
        private int _reconnectionTimeout = 1000 * 10;
        private int _filesReadingTimeout = 1000 * 10;

        private string[] _files;
        private int _fileIndex = -1;
        private MessageSendingState _messageSendingState;

        public BusPublisher(SdrnServerDescriptor serverDescriptor, MMB.MessageConverter messageConverter, ILogger logger)
        {
            this._serverDescriptor = serverDescriptor;
            this._messageConverter = messageConverter;
            this._logger = logger;
            this._connectionFactory = new ConnectionFactory()
            {
                HostName = this._serverDescriptor.RabbitMqHost,
                UserName = this._serverDescriptor.RabbitMqUser,
                Password = this._serverDescriptor.RabbitMqPassword
            };
            if (!string.IsNullOrEmpty(this._serverDescriptor.RabbitMqVirtualHost))
            {
                this._connectionFactory.VirtualHost = this._serverDescriptor.RabbitMqVirtualHost;
            }
            if (!string.IsNullOrEmpty(this._serverDescriptor.RabbitMqPort))
            {
                this._connectionFactory.Port = int.Parse(this._serverDescriptor.RabbitMqPort);
            }

            this._exchangeName = $"{this._serverDescriptor.MessagesExchange}.[v{this._serverDescriptor.ApiVersion}]";
            this._mainTask = new Task(() => this.Process());
            this._mainTask.Start();
        }

        private void Process()
        {
            System.Threading.Thread.Sleep(this._startingTimeout);
            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher.Process", $"The main task is started");

            try
            {
                this.DeclareEnvironment();
                this.ProcessMessages();
            }
            catch(Exception e)
            {
                this._logger.Critical("SdrnDeviceServices", (EventCategory)"Publisher.Process", e);
                this._logger.Critical("SdrnDeviceServices", (EventCategory)"Publisher.Process", "Please fix the problem and restart the service.");
            }
        }

        private void DeclareEnvironment()
        {
            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher", $"The declaring is started");

            var isDeclared = false;
            while(!isDeclared)
            {
                this.EstablisheConnection();
                try
                {
                    using (var channel = this._connection.CreateModel())
                    {
                        this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher.DeclareEnvironment", $"The connection to Rabbit MQ Server was checked successfully: Host = '{this._serverDescriptor.RabbitMqHost}'");

                        var deviceExchange = $"{this._serverDescriptor.MessagesExchange}.[v{this._serverDescriptor.ApiVersion}]";

                        channel.ExchangeDeclare(
                            exchange: deviceExchange,
                            type: "direct",
                            durable: true
                        );

                        this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher.DeclareEnvironment", $"The Exchange was declared successfully: Name = '{deviceExchange}'");

                        var bindings = this._serverDescriptor.QueueBindings.Values;
                        foreach (var binding in bindings)
                        {
                            var routingKey = $"[{this._serverDescriptor.ServerInstance}].[{binding.RoutingKey}]";
                            var queueName = $"{this._serverDescriptor.ServerQueueNamePart}.[{this._serverDescriptor.ServerInstance}].[{binding.RoutingKey}].[v{this._serverDescriptor.ApiVersion}]";

                            channel.QueueDeclare(
                               queue: queueName,
                               durable: true,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                            channel.QueueBind(queueName, deviceExchange, routingKey);

                            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher.DeclareEnvironment", $"The queue was declared successfully: Name = '{queueName}', RoutingKey = '{routingKey}'");
                        }
                    }
                    isDeclared = true;
                }
                catch (Exception e)
                {
                    this._logger.Exception("SdrnDeviceServices", (EventCategory)"Publisher.DeclareEnvironment", e);
                    System.Threading.Thread.Sleep(this._declaringTimeout);
                }
            }
            

            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher.DeclareEnvironment", $"The declaring is finished");
        }

        private void ProcessMessages()
        {
            while(true)
            {
                var fileName = this.GetNextMessageFile();
                if (string.IsNullOrEmpty(fileName))
                {
                    return;
                }
                SendMessage(fileName);
            }
        }

        private void SendMessage(string fileName)
        {
            var isSent = false;

            while(!isSent)
            {
                try
                {
                    var messageData = this.ReadMessageData(fileName);
                    if (messageData != null)
                    {
                        this.SendMessage(messageData, fileName);
                        File.Delete(fileName);
                    }
                    isSent = true;
                }
                catch(Exception)
                {
                    System.Threading.Thread.Sleep(this._sendingTimeout);
                }
            }
        }

        private MessageData ReadMessageData(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }

            try
            {
                var body = File.ReadAllBytes(fileName);
                using (var memoryStream = new MemoryStream(body))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Binder = new LocalBinder();
                    var data = (MessageData)formatter.Deserialize(memoryStream);
                    return data;
                }
            }
            catch(Exception e)
            {
                this._logger.Exception("SdrnDeviceServices", (EventCategory)"Publisher.ReadMessageData", e);
                throw;
            }
        }

        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return Convert.ToInt64((TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds);
        }

        private void SendMessage(MessageData data, string fileName)
        {
            try
            {
                this.EstablishePublishChannel();

                var msgKey = this._serverDescriptor.QueueBindings[data.Message.Type].RoutingKey;
                var routingKey = $"[{this._serverDescriptor.ServerInstance}].[{msgKey}]";

                var props = this._publishChannel.CreateBasicProperties();

                props.Persistent = true;
                props.AppId = "Atdi.WcfServices.Sdrn.Device.dll";
                props.MessageId = data.Message.Id;
                props.Type = data.Message.Type;
                if (!string.IsNullOrEmpty(data.Message.ContentType))
                {
                    props.ContentType = data.Message.ContentType;
                }
                if (!string.IsNullOrEmpty(data.Message.ContentEncoding))
                {
                    props.ContentEncoding = data.Message.ContentEncoding;
                }
                if (!string.IsNullOrEmpty(data.Message.CorrelationId))
                {
                    props.CorrelationId = data.Message.CorrelationId;
                }
                props.Timestamp = new AmqpTimestamp(DateTimeToUnixTimestamp(DateTime.Now));

                props.Headers = new Dictionary<string, object>
                {
                    ["SdrnServer"] = data.Descriptor.SdrnServer,
                    ["SensorName"] = data.Descriptor.SensorName,
                    ["SensorTechId"] = data.Descriptor.EquipmentTechId,
                    ["Created"] = data.Created.ToString("o")
                };

                this._messageSendingState = MessageSendingState.Sending;

                this._publishChannel.BasicPublish(exchange: this._exchangeName,
                                    routingKey: routingKey,
                                    basicProperties: props,
                                    body: data.Message.Body);

                this._publishChannel.WaitForConfirmsOrDie();

                if (this._messageSendingState != MessageSendingState.Sent)
                {
                    switch (this._messageSendingState)
                    {
                        case MessageSendingState.Returned:
                            throw new InvalidOperationException("The message was returned");
                        case MessageSendingState.Nacks:
                            throw new InvalidOperationException("The message was Nacks");
                        case MessageSendingState.Aborted:
                            throw new InvalidOperationException("The message sending was aborted");
                        default:
                            throw new InvalidOperationException("The message was not sent");
                    }
                }
                
                this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher.SendMessage", $"The message was sent successfully: MessageType = '{data.Message.Type}', RoutingKey = '{routingKey}', SDRN Server = '{data.Descriptor.SdrnServer}', Sensor = '{data.Descriptor.SensorName}', File = '{fileName}'");

            }
            catch (Exception e)
            {
                this._logger.Exception("SdrnDeviceServices", (EventCategory)"Publisher.SendMessage", e);
                throw new InvalidOperationException("The object was not sent to server", e);
            }
            finally
            {
                this._messageSendingState = MessageSendingState.None;
            }

        }

        private string GetNextMessageFile()
        {
            if (this._files != null && this._fileIndex + 1 < this._files.Length)
            {
                return this._files[++this._fileIndex];
            }
            this._files = ReadFiles();

            while (this._files == null || this._files.Length == 0)
            {
                System.Threading.Thread.Sleep(this._filesReadingTimeout);
                this._files = ReadFiles();
            }

            this._fileIndex = 0;
            return this._files[this._fileIndex];
        }

        private string[] ReadFiles()
        {
            var files = Directory.GetFiles(this._serverDescriptor.MessagesOutboxFolder, "*.mdata", SearchOption.TopDirectoryOnly);
            if (files != null && files.Length > 0)
            {
                return files.OrderBy(s => s).ToArray();
            }
            return new string[] { };
        }

        public void Dispose()
        {
            if (this._mainTask != null)
            {
                this._mainTask = null;
            }

            if (this._publishChannel != null)
            {
                if (this._publishChannel.IsOpen)
                {
                    this._publishChannel.Close();
                }
                this._publishChannel.Dispose();
                this._publishChannel = null;
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

        private void EstablishePublishChannel()
        {
            EstablisheConnection();
            if (this._publishChannel != null)
            {
                if (this._publishChannel.IsOpen)
                {
                    return;
                }
                this._publishChannel.Dispose();
                this._publishChannel = null;
            }

            this._publishChannel = this._connection.CreateModel();
            this._publishChannel.ModelShutdown += _publishChannel_ModelShutdown;
            this._publishChannel.BasicAcks += _publishChannel_BasicAcks;
            this._publishChannel.BasicNacks += _publishChannel_BasicNacks;
            this._publishChannel.BasicReturn += _publishChannel_BasicReturn;
            this._publishChannel.ConfirmSelect();
            this._messageSendingState = MessageSendingState.None;
        }

        private void _publishChannel_BasicReturn(object sender, RabbitMQ.Client.Events.BasicReturnEventArgs e)
        {
            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher.Channel.BasicReturn", $"The message was returned: ReplyText = '{e.ReplyText}', RoutingKey = '{e.RoutingKey}', Exchange = '{e.Exchange}', Type = '{e.BasicProperties.Type}'");
            if (this._messageSendingState == MessageSendingState.Sending)
            {
                this._messageSendingState = MessageSendingState.Returned;
            }
        }

        private void _publishChannel_BasicNacks(object sender, RabbitMQ.Client.Events.BasicNackEventArgs e)
        {
            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher.Channel.BasicNacks", $"The message was Nacks: DeliveryTag = '{e.DeliveryTag}', Requeue = '{e.Requeue}', Multiple = '{e.Multiple}'");
            if (this._messageSendingState == MessageSendingState.Sending)
            {
                this._messageSendingState = MessageSendingState.Nacks;
            }
        }

        private void _publishChannel_BasicAcks(object sender, RabbitMQ.Client.Events.BasicAckEventArgs e)
        {
            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher.Channel.BasicAcks", $"The message was Acks: DeliveryTag = '{e.DeliveryTag}', Multiple = '{e.Multiple}'");
            if (this._messageSendingState == MessageSendingState.Sending)
            {
                this._messageSendingState = MessageSendingState.Sent;
            }
        }

        private void _publishChannel_ModelShutdown(object sender, ShutdownEventArgs e)
        {
            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher.Channel.ModelShutdown", $"The channel was shutdown: ReplyText = '{e.ReplyText}', Initiator = '{e.Initiator}', ReplyCode = '{e.ReplyCode}'");
            if (this._messageSendingState == MessageSendingState.Sending)
            {
                this._messageSendingState = MessageSendingState.Aborted;
            }
        }

        private void EstablisheConnection()
        {
            var connectionName = $"SDRN Device [{this._serverDescriptor.Instance}][WCF Service][Publisher] #{System.Threading.Thread.CurrentThread.ManagedThreadId}";
            if (this._connection == null)
            {
                var isConnected = false;
                var lastError = "";
                while(!isConnected)
                {
                    try
                    {
                        this._connection = this._connectionFactory.CreateConnection(connectionName);
                        isConnected = this._connection.IsOpen;
                    }
                    catch (Exception e)
                    {
                        if (!e.Message.Equals(lastError))
                        {
                            lastError = e.Message;
                            this._logger.Exception("SdrnDeviceServices", (EventCategory)"Publisher.CreateConnection", e);
                        }
                        
                        System.Threading.Thread.Sleep(this._connectionTimeout);
                    }
                }
                this._connection.ConnectionShutdown += _connection_ConnectionShutdown;
                this._connection.RecoverySucceeded += _connection_RecoverySucceeded;
                this._connection.ConnectionRecoveryError += _connection_ConnectionRecoveryError;

                this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher.EstablisheConnection", $"The connection is opened");
            }

            while (!this._connection.IsOpen)
            {
                System.Threading.Thread.Sleep(this._reconnectionTimeout);
                if (this._connection.IsOpen)
                {
                    this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher.EstablisheConnection", $"The connection is restored");
                }
            }
        }

        private void _connection_ConnectionRecoveryError(object sender, RabbitMQ.Client.Events.ConnectionRecoveryErrorEventArgs e)
        {
            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher.Connection.RecoveryError", $"Reason = '{e.Exception.Message}'");
        }

        private void _connection_RecoverySucceeded(object sender, EventArgs e)
        {
            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher.Connection.Recovery", $"The connection is restored");
        }

        private void _connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"Publisher.Connection.Shutdown", $"The connection is shutdowned: Initiator = '{e.Initiator}', Reason = '{e.ReplyText}'");
        }
    }
}
