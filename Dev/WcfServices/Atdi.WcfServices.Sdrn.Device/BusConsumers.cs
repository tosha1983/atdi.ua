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
using Atdi.DataModels.Sdrns.Device;

namespace Atdi.WcfServices.Sdrn.Device
{
    public class BusConsumers : IDisposable
    {
        private static object _locker = new object();

        private readonly SdrnServerDescriptor _serverDescriptor;
        private readonly MMB.MessageConverter _messageConverter;
        private readonly ILogger _logger;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;

        private int _objectWaitingTimeout = 1000 * 60 * 1;
        private int _objectGettingTimeout = 1000 * 1;
        private int _consumerJoiningTimeout = 1000 * 60;
        private int _declaringTimeout = 1000 * 10;
        private int _startingTimeout = 1000 * 5;
        private int _connectionTimeout = 1000 * 10;
        private int _reconnectionTimeout = 1000 * 10;
        private  Dictionary<string, BusConsumerDescriptor> _consumers;

        private Task _mainTask;
        public BusConsumers(SdrnServerDescriptor serverDescriptor, MMB.MessageConverter messageConverter, ILogger logger)
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

            this.LoadConsumersInfo();

            this._mainTask = new Task(() => this.Process());
            this._mainTask.Start();
        }

        private void LoadConsumersInfo()
        {
            lock(_locker)
            {
                try
                {
                    this._consumers = new Dictionary<string, BusConsumerDescriptor>();

                    if (!Directory.Exists(_serverDescriptor.MessagesInboxFolder))
                    {
                        Directory.CreateDirectory(_serverDescriptor.MessagesInboxFolder);
                    }
                    var files = Directory.GetFiles(_serverDescriptor.MessagesInboxFolder, "*.qdata");
                    if (files == null || files.Length == 0)
                    {
                        return;
                    }

                    for (int i = 0; i < files.Length; i++)
                    {
                        var fileName = files[i];
                        var lines = File.ReadAllLines(fileName);
                        if (lines.Length != 4)
                        {
                            this._logger.Warning("SdrnDeviceServices", (EventCategory)"BusConsumers.LoadConsumersInfo", $"Invalid queue descriptor file: {fileName}");
                            continue;
                        }
                        if (!this._serverDescriptor.DeviceQueueNamePart.Equals(lines[0], StringComparison.OrdinalIgnoreCase))
                        {
                            this._logger.Warning("SdrnDeviceServices", (EventCategory)"BusConsumers.LoadConsumersInfo", $"Invalid queue descriptor file (DeviceQueueNamePart): {fileName}");
                            continue;
                        }
                        if (!this._serverDescriptor.ApiVersion.Equals(lines[1], StringComparison.OrdinalIgnoreCase))
                        {
                            this._logger.Warning("SdrnDeviceServices", (EventCategory)"BusConsumers.LoadConsumersInfo", $"Invalid queue descriptor file (ApiVersion): {fileName}");
                            continue;
                        }
                        if (string.IsNullOrEmpty(lines[2]))
                        {
                            this._logger.Warning("SdrnDeviceServices", (EventCategory)"BusConsumers.LoadConsumersInfo", $"Invalid queue descriptor file (SensorName): {fileName}");
                            continue;
                        }
                        if (string.IsNullOrEmpty(lines[3]))
                        {
                            this._logger.Warning("SdrnDeviceServices", (EventCategory)"BusConsumers.LoadConsumersInfo", $"Invalid queue descriptor file (TechId): {fileName}");
                            continue;
                        }
                        var descriptor = new BusConsumerDescriptor
                        {
                            SensorName = lines[2],
                            TechId = lines[3]
                        };

                        this._consumers.Add(descriptor.ToString(), descriptor);
                    }
                }
                catch(Exception e)
                {
                    this._logger.Exception("SdrnDeviceServices", (EventCategory)"BusConsumers.LoadConsumersInfo", e);
                }
            }
        }

        private void Process()
        {
            System.Threading.Thread.Sleep(this._startingTimeout);
            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"BusConsumers.Process", $"The process is started");

            try
            {
                var canceled = false;
                while(!canceled)
                {
                    var descriptors = _consumers.Values.Where(d => !d.IsJoined).ToArray();
                    foreach (var descriptor in descriptors)
                    {
                        var deviceQueueName = this.DeclareQueue(descriptor);
                        this.JoinConsumer(descriptor, deviceQueueName);
                    }
                    System.Threading.Thread.Sleep(this._consumerJoiningTimeout);
                }
            }
            catch(Exception e)
            {
                this._logger.Critical("SdrnDeviceServices", (EventCategory)"BusConsumers.Process", e);
                this._logger.Critical("SdrnDeviceServices", (EventCategory)"BusConsumers.Process", "Please fix the problem and restart the service.");
            }
        }

        private void JoinConsumer(BusConsumerDescriptor descriptor, string deviceQueueName)
        {
            IModel channel = null;
            try
            {
                this.EstablisheConnection();
                channel = _connection.CreateModel();
                var tag = descriptor.ToString();
                var consumer = new BusConsumer(tag, channel, this._serverDescriptor, this._logger);
                descriptor.IsJoined = true;
                descriptor.Consumer = consumer;
                channel.BasicConsume(deviceQueueName, false, consumer);

                this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"BusConsumers.JoinConsumer", $"The consumer is joined: tag = '{tag}', queue = {deviceQueueName}");
            }
            catch (Exception e)
            {
                this._logger.Exception("SdrnDeviceServices", (EventCategory)"BusConsumers.JoinConsumer", e);
                descriptor.IsJoined = false;
                descriptor.Consumer = null;
                if (channel != null)
                {
                    if (channel.IsOpen)
                    {
                        channel.Close();
                    }
                    channel.Dispose();
                }
            }
            
        }

        private string DeclareQueue(BusConsumerDescriptor descriptor)
        {
            var deviceQueueName = $"{this._serverDescriptor.DeviceQueueNamePart}.[{descriptor.SensorName}].[{descriptor.TechId}].[v{this._serverDescriptor.ApiVersion}]";

            var isDeclared = false;
            while(!isDeclared)
            {
                this.EstablisheConnection();
                try
                {
                    using (var channel = this._connection.CreateModel())
                    {
                        channel.QueueDeclare(
                            queue: deviceQueueName,
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        channel.Close();
                    }
                    isDeclared = true;
                }
                catch (Exception e)
                {
                    this._logger.Exception("SdrnDeviceServices", (EventCategory)"RabbitMQ.DeclareQueue", e);
                    System.Threading.Thread.Sleep(this._declaringTimeout);
                }
            }
            

            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"BusConsumers.DeclareQueue", $"The declaring queue is finished");
            return deviceQueueName;
        }
     
        public void DeclareSensor(string sensorName, string techId)
        {
            if (string.IsNullOrEmpty(sensorName))
            {
                throw new ArgumentException("Undefined", nameof(sensorName));
            }

            if (string.IsNullOrEmpty(techId))
            {
                throw new ArgumentException("Undefined", nameof(techId));
            }

            var descriptor = new BusConsumerDescriptor
            {
                SensorName = sensorName,
                TechId = techId
            };

            if (this._consumers.ContainsKey(descriptor.ToString()))
            {
                return;
            }

            lock(_locker)
            {
                if (this._consumers.ContainsKey(descriptor.ToString()))
                {
                    return;
                }

                var fileName = "q_" + (this._consumers.Count + 1).ToString() + ".qdata";
                var fullPath = Path.Combine(this._serverDescriptor.MessagesInboxFolder, fileName);
                var lines = new string[]
                {
                    this._serverDescriptor.DeviceQueueNamePart,
                    this._serverDescriptor.ApiVersion,
                    descriptor.SensorName,
                    descriptor.TechId
                };

                File.WriteAllLines(fullPath, lines);
                _consumers.Add(descriptor.ToString(), descriptor);
            }
        }

        private string[] ReadFiles(string typeName)
        {
            var directory = Path.Combine(this._serverDescriptor.MessagesInboxFolder, BusConsumer.PrepareTypeForFolderName(typeName));
            var files = Directory.GetFiles(directory, "*.mdata", SearchOption.TopDirectoryOnly);
            if (files != null && files.Length > 0)
            {
                return files.OrderBy(s => s).ToArray();
            }
            return new string[] { };
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
            catch (Exception e)
            {
                this._logger.Exception("SdrnDeviceServices", (EventCategory)"BusConsumers.ReadMessageData", e);
                throw;
            }
        }

        public TObject TryGetObject<TObject>(SensorDescriptor descriptor, string messageType, out string token, bool isAutoAck = false)
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

                var files = this.ReadFiles(messageType);
                for (int i = 0; i < files.Length; i++)
                {
                    var fileName = files[i];
                    var data = this.ReadMessageData(fileName);
                    if (data == null)
                    {
                        continue;
                    }
                    if (!messageType.Equals(data.Message.Type, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    if (!descriptor.SensorName.Equals(data.Descriptor.SensorName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    if (!descriptor.EquipmentTechId.Equals(data.Descriptor.EquipmentTechId, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    token = data.Message.Id;
                    var messageObject = this._messageConverter.Deserialize(data.Message);
                    var resultObject = (TObject)messageObject.Object;

                    if (isAutoAck)
                    {
                        File.Delete(fileName);
                    }

                    return resultObject;
                }

                return default(TObject);
            }
            catch (Exception e)
            {
                this._logger.Exception("SdrnDeviceServices", (EventCategory)"BusConsumers.TryGetObject", e);
                throw new InvalidOperationException("The object was not gotten from server", e);
            }
        }

        public void TryAckMessage(string messageType, byte[] token)
        {
            if (string.IsNullOrEmpty(messageType))
            {
                throw new ArgumentException("Invalid argument", nameof(messageType));
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

                var files = this.ReadFiles(messageType);
                for (int i = 0; i < files.Length; i++)
                {
                    var fileName = files[i];
                    var data = this.ReadMessageData(fileName);
                    if (data == null)
                    {
                        continue;
                    }
                    if (!messageType.Equals(data.Message.Type, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    if (!messageId.Equals(data.Message.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    File.Delete(fileName);
                }
            }
            catch (Exception e)
            {
                this._logger.Exception("SdrnDeviceServices", (EventCategory)"BusConsumers.TryAckMessage", e);
                throw new InvalidOperationException("The message was not ack", e);
            }
        }

        public TObject WaitObject<TObject>(SensorDescriptor descriptor, string messageType)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }
            if (string.IsNullOrEmpty(messageType))
            {
                throw new ArgumentException("message", nameof(messageType));
            }
            var timer = System.Diagnostics.Stopwatch.StartNew();

            while(true)
            {
                var token = string.Empty;
                var result = this.TryGetObject<TObject>(descriptor, messageType, out token, true);
                timer.Stop();
                if (result != null)
                {
                    return result;
                }
                if (timer.ElapsedMilliseconds >= this._objectWaitingTimeout)
                {
                    throw new TimeoutException();
                }
                timer.Start();

                System.Threading.Thread.Sleep(this._objectGettingTimeout);
            }
        }
        public void Dispose()
        {
            if (_consumers != null)
            {
                var descriptors = _consumers.Values.Where(d => d.IsJoined && d.Consumer != null).ToArray();
                foreach (var descriptor in descriptors)
                {
                    descriptor.Consumer.Dispose();
                }
                this._consumers = null;
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

        private void EstablisheConnection()
        {
            var connectionName = $"SDRN Device [{this._serverDescriptor.Instance}][WCF Service][Consumers] #{System.Threading.Thread.CurrentThread.ManagedThreadId}";
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
                            this._logger.Exception("SdrnDeviceServices", (EventCategory)"BusConsumers.CreateConnection", e);
                        }
                        
                        System.Threading.Thread.Sleep(this._connectionTimeout);
                    }
                }
                this._connection.ConnectionShutdown += _connection_ConnectionShutdown;
                this._connection.RecoverySucceeded += _connection_RecoverySucceeded;
                this._connection.ConnectionRecoveryError += _connection_ConnectionRecoveryError;

                this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"BusConsumers.EstablisheConnection", $"The connection is opened");
            }

            while (!this._connection.IsOpen)
            {
                System.Threading.Thread.Sleep(this._reconnectionTimeout);
                if (this._connection.IsOpen)
                {
                    this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"BusConsumers.EstablisheConnection", $"The connection is restored");
                }
            }
        }

        private void _connection_ConnectionRecoveryError(object sender, RabbitMQ.Client.Events.ConnectionRecoveryErrorEventArgs e)
        {
            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"BusConsumers.ConnectionRecoveryError", $"Reason = '{e.Exception.Message}'");
        }

        private void _connection_RecoverySucceeded(object sender, EventArgs e)
        {
            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"BusConsumers.ConnectionRecovery", $"The connection is restored");
        }

        private void _connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            this._logger.Verbouse("SdrnDeviceServices", (EventCategory)"BusConsumers.ConnectionShutdown", $"The connection is shutdowned: Initiator = '{e.Initiator}', Reason = '{e.ReplyText}'");
        }
    }
}
