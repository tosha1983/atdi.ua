using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Api.DataBus;
using Atdi.Modules.AmqpBroker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.Api.DataBus
{
    internal sealed class Dispatcher : IDispatcher
    {
        private readonly object _connectionLocker = new object();
        private readonly object _mainLocker = new object();

        private readonly ConnectionConfig _amqpConfig;
        private readonly ConnectionFactory _amqpConnectionFactory;
        private Connection _amqpConnection;
        private readonly HandlersHost _handlersHost;
        private readonly IBusConfig _config;
        private readonly IMessageHandlerResolver _handlerResolver;
        private readonly MessagePacker _messagePacker;
        private readonly BusLogger _logger;
        private Task _task;
        private CancellationTokenSource _tokenSource;
        
        private readonly Dictionary<string, QueueDescriptor> _queues;
        /// <summary>
        /// Время ожидания завершения таска процесса после попытки его отменить
        /// </summary>
        private int _canceledTaskTimeout = 30 * 1000;
        /// <summary>
        /// Таймаут между неудачными попытками процесинга обработчика
        /// </summary>
        private int _processingDescriptorTimeout = 20 * 1000;

        public Dispatcher(IBusConfig config, ConnectionFactory amqpConnectionFactory, IMessageHandlerResolver handlerResolver, MessagePacker messagePacker, BusLogger logger)
        {
            this._config = config;
            this._amqpConnectionFactory = amqpConnectionFactory;
            this._handlerResolver = handlerResolver;
            this._messagePacker = messagePacker;
            this._logger = logger;
            this._queues = new Dictionary<string, QueueDescriptor>();
            this._handlersHost = new HandlersHost();
            this._amqpConfig = new ConnectionConfig
            {
                AutoRecovery = true,
                ConnectionName = $"DataBus.[{_config.Name}].[{_config.Address}].[v{_config.ApiVersion}].Dispatcher",
                HostName = _config.Host,
                VirtualHost = _config.VirtualHost,
                Port = _config.Port,
                UserName = _config.User,
                Password = _config.Password
            };
        }

        public DispatcherState State { get; set; }

        public void Activate()
        {
            lock (_mainLocker)
            {
                if (this._task != null)
                {
                    return;
                }
                this._tokenSource = new CancellationTokenSource();
                State = DispatcherState.Activated;
                this._task = Task.Run(() => Process());
            }
        }

        private void Process()
        {

            this._logger.Verbouse(BusContexts.ConsumerProcessing, $"The Consumer Processing Thread is started", this);

            try
            {
                this.ProcessDeclare();
                this.ProcessConsumers();
            }
            catch (OperationCanceledException)
            {
                this._logger.Verbouse(BusContexts.ConsumerProcessing, $"The Consumer Processing Thread was canceled", this);
            }
            catch (ThreadAbortException e)
            {
                Thread.ResetAbort();
                this._logger.Exception(BusContexts.ConsumerProcessing, "Abort the thread of the Consumer Processing. Please fix the problem and restart the service.", e, this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BusContexts.ConsumerProcessing, "Abort the thread of the Consumer Processing. Please fix the problem and restart the service.", e, this);
            }
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
                this._logger.Exception(BusContexts.ConnectionEstablishing, $"An error occurred while establishing channel to Data Bus: {_amqpConfig}", e, this);
                result = false;
            }
            return result;
        }

        private bool TryDeclareQueue(Channel channel, string queueName, string bindKey, string exchegeName)
        {
            var result = false;
            try
            {
                channel.DeclareDurableQueue(queueName, exchegeName, bindKey);
                result = true;
            }
            catch (Exception e)
            {
                this._logger.Exception(BusContexts.ConnectionEstablishing, $"An error occurred during the configuration of the Data Bus: Declaring Queue = '{queueName}'", e, this);
                result = false;
            }
            return result;
        }

        private bool TryDeclareQueue(Channel channel, string queueName)
        {
            var result = false;
            try
            {
                channel.DeclareDurableQueue(queueName);
                result = true;
            }
            catch (Exception e)
            {
                this._logger.Exception(BusContexts.ConnectionEstablishing, $"An error occurred during the configuration of the Data Bus: Declaring Queue = '{queueName}'", e, this);
                result = false;
            }
            return result;
        }

        private void ProcessDeclare()
        {
            while(true)
            {
                if (this.TryToEstablishConnection())
                {
                    if (this.TryToEstablishChannel(out Channel channel))
                    {
                        var exchangeName = $"E.DataBus.[{_config.Name}].Common.[v{_config.ApiVersion}]";
                        if (this.TryDeclareExchenge(channel, exchangeName))
                        {
                            var localExchangeName = $"E.DataBus.[{_config.Name}].Local.[{_config.Address}].[v{_config.ApiVersion}]";
                            if (this.TryDeclareExchenge(channel, localExchangeName))
                            {
                                var rejectedQueueName = $"Q.DataBus.[{_config.Name}].[{_config.Address}].Rejected.[v{_config.ApiVersion}]";
                                if (this.TryDeclareQueue(channel, rejectedQueueName, "RK.[Rejected]", localExchangeName))
                                {
                                    var unprocessedQueueName = $"Q.DataBus.[{_config.Name}].[{_config.Address}].Unprocessed.[v{_config.ApiVersion}]";
                                    if (this.TryDeclareQueue(channel, unprocessedQueueName, "RK.[Unprocessed]", localExchangeName))
                                    {
                                        channel.Dispose();
                                        return; // finished
                                    }
                                    else
                                    {
                                        this._logger.Error(0, BusContexts.MessageSending, $"The local unprocessed queue of the Data Hub is not established: {_amqpConfig}", this);
                                    }
                                }
                                else
                                { 
                                    this._logger.Error(0, BusContexts.MessageSending, $"The local rejected queue of the Data Hub is not established: {_amqpConfig}", this);
                                }
                            }
                            else
                            {
                                this._logger.Error(0, BusContexts.ConsumerProcessing, $"The local exchenge of the Data Hub is not established: {_amqpConfig}", this);
                            }
                        }
                        else
                        {
                            this._logger.Error(0, BusContexts.ConsumerProcessing, $"The common exchenge of the Data Hub is not established: {_amqpConfig}", this);
                        }
                    }
                    else
                    {
                        channel.Dispose();
                        this._logger.Error(0, BusContexts.ConsumerProcessing, $"The channel of the Data Hub is not established: {_amqpConfig}", this);
                    }
                }
                else
                {
                    this._logger.Error(0, BusContexts.ConsumerProcessing, $"The connection to the Data Hub is not established: {_amqpConfig}", this);
                }

                this._tokenSource.Token.ThrowIfCancellationRequested();

                Thread.Sleep(this._processingDescriptorTimeout);
            }
        }

        private void ProcessConsumers()
        {
            foreach (var queueDescriptor in this._queues.Values)
            {
                var isDone = false;
                while(!isDone)
                {
                    isDone = this.TryProcessConsumer(queueDescriptor);

                    if (!isDone)
                    {
                        this._tokenSource.Token.ThrowIfCancellationRequested();

                        Thread.Sleep(this._processingDescriptorTimeout);
                    }
                }
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
                            this._logger.Exception(BusContexts.ConnectionEstablishing, $"An error occurred while establishing connection to Data Bus: {_amqpConfig}", e, this);
                            return false;
                        }
                    }

                }

            }

            var state = this._amqpConnection.IsOpen;
            return state;
        }
        private bool TryDeclareExchenge(Channel channel, string exchegeName)
        {
            var result = false;
            try
            {
                channel.DeclareDurableDirectExchange(exchegeName);
                result = true;
            }
            catch (Exception e)
            {
                this._logger.Exception(BusContexts.ConsumerProcessing, $"An error occurred during the configuration of the Data Bus: Declaring Exchange = '{exchegeName}'", e, this);
                result = false;
            }
            return result;
        }

        private bool TryProcessConsumer(QueueDescriptor queueDescriptor)
        {
            if (!this.TryToEstablishConnection())
            {
                this._logger.Error(0, BusContexts.ConsumerProcessing, $"The connection to the Data Hub is not established: {_amqpConfig}", this);
                return false;
            }

            if (!this.TryToEstablishChannel(out Channel channel))
            {
                this._logger.Error(0, BusContexts.ConsumerProcessing, $"The channel of the Data Hub is not established: {_amqpConfig}", this);
                return false;
            }

            if (!this.TryDeclareQueue(channel, queueDescriptor.Name))
            {
                this._logger.Error(0, BusContexts.ConsumerProcessing, $"The connection to the Data Hub is not established: {_amqpConfig}", this);
                channel.Dispose();
                return false;
            }
            channel.Dispose();

            try
            {
                queueDescriptor.DeliveryHandler.Join(this._amqpConnection);
                return true;
            }
            catch(Exception e)
            {
                this._logger.Exception(BusContexts.ConsumerProcessing, $"An error occurred while joining consumer to queue {queueDescriptor.Name}", e, this);
            }

            return false;
        }

        public void Deactivate()
        {
            lock (_mainLocker)
            {
                if (this._task == null)
                {
                    return;
                }
                this.State = DispatcherState.Deactivated;
                this._tokenSource.Cancel();
                if (!this._task.Wait(_canceledTaskTimeout))
                {
                    this._logger.Error(0, BusContexts.ConsumerProcessing, "Timeout waiting for file processing to exceeded", this);
                }
                foreach (var queue in _queues.Values)
                {
                    try
                    {
                        queue.DeliveryHandler.UnJoin();
                    }
                    catch (Exception e)
                    {
                        this._logger.Exception(BusContexts.ConsumerProcessing, $"An error occurred while unjoining consumer from queue {queue.Name}", e, this);
                    }
                }

                if (_amqpConnection != null)
                {
                    _amqpConnection.Dispose();
                    _amqpConnection = null;
                }
                this._tokenSource = null;
                this._task = null;
            }
        }

        public void Dispose()
        {
            this.Deactivate();
            foreach (var queue in _queues.Values)
            {
                queue.DeliveryHandler.Dispose();
            }
        }

        public void RegistryHandler(Type handlerType)
        {
            lock(_mainLocker)
            {
                var descriptor = this._handlersHost.RegistryHandler(handlerType);

                var queueName = AmqpPublisher.BuildQueueName(
                    descriptor.MessageTypeInstance.QueueType,
                    descriptor.MessageTypeInstance.Name,
                    _config.Address,
                    _config.Name,
                    descriptor.MessageTypeInstance.SpecificQueue,
                    _config.ApiVersion);

                if (this._queues.ContainsKey(queueName))
                {
                    return;
                }

                var deliveryHandler = new AmqpDeliveryHandler(this._config, queueName, this._handlersHost, this._handlerResolver, this._messagePacker, this._logger);
                var ququeDescriptor = new QueueDescriptor(queueName, deliveryHandler);

                this._queues.Add(queueName, ququeDescriptor);
            }
        }
    }
}
