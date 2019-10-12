using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Modules.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atdi.Modules.AmqpBroker;
using Atdi.Modules.Sdrn.DeviceBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal class MessageDispatcher : IMessageDispatcher
    {
        private readonly object _connectionLocker = new object();
        private readonly object _mainLocker = new object();
        private readonly BusLogger _logger;
        private readonly DeviceBusConfig _config;
        private readonly ConnectionFactory _amqpConnectionFactory;
        private Connection _amqpConnection;
        private readonly BusMessagePacker _messagePacker;
        private readonly ConnectionConfig _amqpConfig;
        private readonly Dictionary<string, List<MessageHandlerDescriptor>> _handlers;
        private MessageDispatcherState _state;
        private AmqpDeliveryHandler _amqpConsumer;
        private Thread _thread;
        private CancellationTokenSource _tokenSource;

        /// <summary>
        /// Время ожидания завершения таска процесса после полпытке его отменить
        /// </summary>
        private int _canceledTaskTimeout = 5 * 1000;
        /// <summary>
        /// Таймаут между неудачными попытками процесинга обработчика
        /// </summary>
        private int _declarationProcessingTimeout = 20 * 1000;
        /// <summary>
        /// Таймаут между неудачными попытками процесинга присоединения консюмера к шине
        /// </summary>
        private int _consumerAttachmentProcessingTimeout = 20 * 1000;

        private int _objectWaitingTimeout = 1000 * 60 * 1;
        private int _objectGettingTimeout = 1000 * 1;

        internal MessageDispatcher(string tag, DeviceBusConfig config, ConnectionFactory amqpConnectionFactory, BusMessagePacker messagePacker, BusLogger logger)
        {
            this.Tag = tag;
            this._logger = logger;
            this._config = config;
            this._amqpConnectionFactory = amqpConnectionFactory;
            this._messagePacker = messagePacker;

            this._amqpConfig = new ConnectionConfig
            {
                AutoRecovery = true,
                ConnectionName = $"Sensor:[{_config.DeviceBusClient}].[{_config.SdrnDeviceSensorName}].[{_config.SdrnDeviceSensorTechId}].Dispatcher",
                HostName = _config.RabbitMqHost,
                VirtualHost = _config.RabbitMqVirtualHost,
                Port = _config.RabbitMqPort,
                UserName = _config.RabbitMqUser,
                Password = _config.RabbitMqPassword
            };

            this._state = MessageDispatcherState.Deactivated;
            this._handlers = new Dictionary<string, List<MessageHandlerDescriptor>>();
            //this._consumer = this._rabbitBus.CreateConsumer(this._config.BuildDeviceQueueName(), this.Tag, this);
        }

        public string Tag { get; }

        public MessageDispatcherState State => this._state;

        public void Activate()
        {
            lock (_mainLocker)
            {
                if (this._state == MessageDispatcherState.Activated)
                {
                    return;
                }
                this._tokenSource = new CancellationTokenSource();
                if (this._thread == null)
                {
                    this._thread = new Thread(this.Process)
                    {
                        Name = "Atdi.DeviceBus.ConsumerProcessing"
                    };
                    this._thread.Start();
                }
                this._state = MessageDispatcherState.Activated;
            }
        }

        public void Deactivate()
        {
            lock (_mainLocker)
            {
                if (this._state == MessageDispatcherState.Deactivated)
                {
                    return;
                }

                if (this._tokenSource != null)
                {
                    this._tokenSource.Cancel();
                    Thread.Sleep(_canceledTaskTimeout);
                }
                this._thread = null;

                if (_amqpConsumer != null)
                {
                    _amqpConsumer.Detach();
                    _amqpConsumer = null;
                }

                if (_amqpConnection != null)
                {
                    _amqpConnection.Dispose();
                    _amqpConnection = null;
                }
                this._tokenSource = null;
                this._state = MessageDispatcherState.Deactivated;
            }
        }

        private void Process()
        {
            this._logger.Info("DeviceBus.ConsumerProcessing", $"The consumer processing thread was started: Name='{Thread.CurrentThread.Name}'", this);

            try
            {
                this.ProcessDeclaration();
                this.ProcessConsumerAttachment();

                this._logger.Info("DeviceBus.ConsumerProcessing",
                    $"The Consumers Processing Thread is finished normally", this);
            }
            catch (OperationCanceledException)
            {
                this._logger.Verbouse("DeviceBus.ConsumerProcessing", $"The consumers processing thread was canceled: Name='{Thread.CurrentThread.Name}'",
                    this);
            }
            catch (ThreadAbortException e)
            {
                Thread.ResetAbort();
                this._logger.Exception("DeviceBus.ConsumerProcessing",
                    $"Abort the thread of the Consumer Processing. Please fix the problem and restart the service: Name='{Thread.CurrentThread.Name}'", e,
                    this);
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.ConsumerProcessing",
                    $"Abort the thread of the Consumer Processing. Please fix the problem and restart the service: Name='{Thread.CurrentThread.Name}'", e,
                    this);
            }
            finally
            {
                this._thread = null;
            }
        }

        private void ProcessConsumerAttachment()
        {
            var isDone = false;
            while (!isDone)
            {
                isDone = this.TryAttachConsumer();

                if (!isDone)
                {
                    this._tokenSource.Token.ThrowIfCancellationRequested();

                    Thread.Sleep(this._consumerAttachmentProcessingTimeout);
                }
            }
        }

        private bool TryAttachConsumer()
        {
            var deviceQueue = _config.BuildDeviceQueueName();
            if (!this.TryToEstablishConnection())
            {
                this._logger.Error("DeviceBus.ConsumerAttachment", $"The connection to the Data Hub is not established: {_amqpConfig}", this);
                return false;
            }

            try
            {
                var consumer = new AmqpDeliveryHandler(this.Tag, _config, deviceQueue, _handlers, _messagePacker, _logger);
                consumer.Attach(this._amqpConnection);
                _amqpConsumer = consumer;
                this._logger.Info("DeviceBus.ConsumerAttachment", $"The consumer is attached: Queue='{deviceQueue}'", this);
                return true;
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.ConsumerAttachment", $"An error occurred while attaching consumer to queue: Queue='{deviceQueue}'", e, this);
            }

            return false;
        }

        private void ProcessDeclaration()
        {
            while (true)
            {
                if (this.TryToEstablishConnection())
                {
                    if (this.TryToEstablishChannel(out var channel))
                    {
                        var deviceQueueName = _config.BuildDeviceQueueName();
                        if (this.TryDeclareQueue(channel, deviceQueueName))
                        {
                            channel.Dispose();
                            return; // finished
                        }
                        channel.Dispose();
                    }
                    else
                    {
                        channel?.Dispose();
                        this._logger.Error(0, "DeviceBus.Configuration", $"The channel of the Data Hub is not established: {_amqpConfig}", this);
                    }
                }
                else
                {
                    this._logger.Error(0, "DeviceBus.Configuration", $"The connection to the Data Hub is not established: {_amqpConfig}", this);
                }

                this._tokenSource.Token.ThrowIfCancellationRequested();

                Thread.Sleep(this._declarationProcessingTimeout);
            }
        }

        private bool TryToEstablishConnection()
        {
            if (this._amqpConnection == null)
            {
                lock (_connectionLocker)
                {
                    if (this._amqpConnection == null)
                    {
                        try
                        {
                            var connection = this._amqpConnectionFactory.Create(_amqpConfig);
                            this._amqpConnection = connection;
                        }
                        catch (Exception e)
                        {
                            this._logger.Exception("DeviceBus.ConnectionEstablishment", $"An error occurred while establishing connection to Device Bus: {_amqpConfig}", e, this);
                            return false;
                        }
                    }

                }

            }

            var state = this._amqpConnection.IsOpen;
            return state;
        }

        private bool TryToEstablishChannel(out Channel channel)
        {
            var result = false;
            channel = null;
            try
            {
                channel = _amqpConnection.CreateChannel();
                channel.EstablishChannel();
                result = channel.IsOpen;
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.ChannelEstablishment", $"An error occurred while establishing channel to Device Bus: {_amqpConfig}", e, this);
                result = false;
            }
            return result;
        }

        private bool TryDeclareQueue(Channel channel, string queueName)
        {
            bool result;
            try
            {
                channel.DeclareDurableQueue(queueName);
                result = true;
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.Configuration", $"An error occurred during the configuration of the Device Bus: Queue = '{queueName}'", e, this);
                result = false;
            }
            return result;
        }

        public void Dispose()
        {
            if (this._state == MessageDispatcherState.Activated)
            {
                this.Deactivate();
            }
        }

        public void RegistryHandler<TObject>(string messageType, Action<IReceivedMessage<TObject>> handler)
        {
            this.RegistryHandler(new ActionMessageHandler<TObject>(messageType, handler));
        }

        public void RegistryHandler<TObject>(IMessageHandler<TObject> handler)
        {
            if (this._state == MessageDispatcherState.Activated)
            {
                throw new InvalidOperationException("Dispatcher is activated");
            }

            var handlerDescriptor = new MessageHandlerDescriptor(handler, handler.MessageType);

            if (!_handlers.ContainsKey(handlerDescriptor.MessageType))
            {
                var list = new List<MessageHandlerDescriptor>
                {
                    handlerDescriptor
                };
                _handlers[handlerDescriptor.MessageType] = list;
            }
            else
            {
                var list = _handlers[handlerDescriptor.MessageType];
                list.Add(handlerDescriptor);
            }
        }

        public void TryAckMessage(IMessageToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            try
            {
                var files = this.ReadFiles(token.Type);
                foreach (var fileName in files)
                {
                    var message = this.ReadMessageFromFile(fileName);
                    if (message == null)
                    {
                        continue;
                    }
                    if (!token.Type.Equals(message.Type, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    if (!token.Id.Equals(message.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    if (!_config.SdrnDeviceSensorName.Equals(message.Sensor, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    if (!_config.SdrnDeviceSensorName.Equals(message.TechId, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    if (!_config.SdrnServerInstance.Equals(message.Server, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    File.Delete(fileName);
                }
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.Acknowledgment", e, this);
                throw new InvalidOperationException("The message was not acknowledged", e);
            }
        }

        public IReceivedMessage<TObject> TryGetObject<TObject>(string messageType, string correlationId = null, bool isAutoAck = false)
        {
            if (string.IsNullOrEmpty(messageType))
            {
                throw new ArgumentException("Invalid argument", nameof(messageType));
            }

            try
            {
                var files = this.ReadFiles(messageType);
                foreach (var fileName in files)
                {
                    var source = this.ReadMessageFromFile(fileName);
                    if (source == null)
                    {
                        continue;
                    }
                    if (!messageType.Equals(source.Type, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    if (!_config.SdrnDeviceSensorName.Equals(source.Sensor, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    if (!_config.SdrnDeviceSensorTechId.Equals(source.TechId, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    if (!_config.SdrnServerInstance.Equals(source.Server, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(correlationId))
                    {
                        if (!correlationId.Equals(source.CorrelationId))
                        {
                            continue;
                        }
                    }

                    var messageObject = BusMessagePacker.Unpack(source.ContentType, source.ContentEncoding, source.Protocol,
                        source.Body, source.BodyAqName, _config.DeviceBusSharedSecretKey);

                    var messageToken = new MessageToken(source.Id, source.Type);

                    var result = (IReceivedMessage<TObject>)Activator.CreateInstance(
                        MessageHandlerDescriptor.MakeReceivedMessageGenericType<TObject>(),
                        new[]
                        {
                            messageToken,
                            messageObject,
                            DateTime.Now,
                            source.CorrelationId
                        });

                    if (isAutoAck)
                    {
                        File.Delete(fileName);
                    }

                    return result;
                }

                return default(IReceivedMessage<TObject>);
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.Pulling", e, this);
                throw new InvalidOperationException("The object was not gotten from server", e);
            }
        }

        private BufferedMessage ReadMessageFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                this._logger.Error(0, "DeviceBus.Pulling", $"The file not found: Name='{fileName}'", this);
                return null;
            }

            try
            {
                var fileExtenstion = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(fileExtenstion))
                {
                    this._logger.Error(0, "DeviceBus.Pulling", $"The file does not contains an extension: Name='{fileName}'", this);
                    return null;
                }

                BufferedMessage message;
                IContentTypeConvertor convertor;

                switch (fileExtenstion.ToLower())
                {
                    case ".json":
                        var json = File.ReadAllText(fileName);
                        convertor = BusMessagePacker.GetConvertor(ContentType.Json);
                        message = (BufferedMessage)convertor.Deserialize(json, typeof(BufferedMessage));
                        break;
                    case ".xml":
                        var xml = File.ReadAllText(fileName);
                        convertor = BusMessagePacker.GetConvertor(ContentType.Xml);
                        message = (BufferedMessage)convertor.Deserialize(xml, typeof(BufferedMessage));
                        break;
                    case ".bin":
                        var binary = File.ReadAllBytes(fileName);
                        convertor = BusMessagePacker.GetConvertor(ContentType.Binary);
                        message = (BufferedMessage)convertor.Deserialize(binary, typeof(BufferedMessage));
                        break;
                    default:
                        this._logger.Error(0, "DeviceBus.Pulling", $"The file contains an unsupported extension: Name='{fileName}'", this);
                        return null;
                }

                return message;
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.Pulling", $"An error occurred while reading the file: Name='{fileName}'", e, this);
                throw;
            }
        }

        private string[] ReadFiles(string typeName)
        {
            var path = string.Intern(Path.Combine(this._config.InboxBufferConfig.Folder, AmqpDeliveryHandler.PrepareTypeForFolderName(typeName)));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
            return files.Length > 0 ? files.OrderBy(s => s).ToArray() : new string[] { };
        }

        public IReceivedMessage<TObject> WaitObject<TObject>(string messageType, string correlationId = null)
        {
            if (string.IsNullOrEmpty(messageType))
            {
                throw new ArgumentException("message", nameof(messageType));
            }

            var timer = System.Diagnostics.Stopwatch.StartNew();
            while (true)
            {
                var result = this.TryGetObject<TObject>(messageType, correlationId, true);
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
                Thread.Sleep(this._objectGettingTimeout);
            }
        }
    }
}
