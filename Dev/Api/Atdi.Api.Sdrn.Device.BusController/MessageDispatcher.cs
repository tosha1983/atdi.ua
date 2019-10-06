using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Modules.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
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
            this._logger.Info("DeviceBus.ConsumerProcessing", $"The Consumer Processing Thread starting ...", this);

            try
            {
                this.ProcessDeclaration();
                this.ProcessConsumerAttachment();

                this._logger.Info("DeviceBus.ConsumerProcessing",
                    $"The Consumers Processing Thread is finished normally", this);
            }
            catch (OperationCanceledException)
            {
                this._logger.Verbouse("DeviceBus.ConsumerProcessing", $"The Consumers Processing Thread was canceled",
                    this);
            }
            catch (ThreadAbortException e)
            {
                Thread.ResetAbort();
                this._logger.Exception("DeviceBus.ConsumerProcessing",
                    "Abort the thread of the Consumer Processing. Please fix the problem and restart the service.", e,
                    this);
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.ConsumerProcessing",
                    "Abort the thread of the Consumer Processing. Please fix the problem and restart the service.", e,
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

        

        //private object DeserializeObjectFromByteArray(byte[] source, Type type)
        //{
        //    if (source == null)
        //    {
        //        return null;
        //    }

        //    var json = Encoding.UTF8.GetString(source);
        //    var result = JsonConvert.DeserializeObject(json, type);

        //    return result;
        //}

        //private static string GetHeaderValue(IBasicProperties properties, string key)
        //{
        //    var headers = properties.Headers;
        //    if (headers == null)
        //    {
        //        return null;
        //    }
        //    if (!headers.ContainsKey(key))
        //    {
        //        return null;
        //    }
        //    return Convert.ToString(Encoding.UTF8.GetString(((byte[])headers[key])));
        //}
    }
}
