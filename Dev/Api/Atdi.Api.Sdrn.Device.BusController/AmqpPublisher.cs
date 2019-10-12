
using Atdi.Modules.AmqpBroker;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using Atdi.Modules.Sdrn.DeviceBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal sealed class AmqpPublisher : IDisposable
    {
        private readonly object _connectionLocker = new object();

        private readonly ConnectionConfig _amqpConfig;
        private readonly DeviceBusConfig _config;
        private readonly ConnectionFactory _amqpConnectionFactory;
        private readonly BusLogger _logger;
        private Connection _amqpConnection;
        private readonly ConcurrentDictionary<int, Channel> _amqpChannels;

        public AmqpPublisher(DeviceBusConfig config, ConnectionFactory amqpConnectionFactory, BusLogger logger)
        {
            this._config = config;
            this._amqpConnectionFactory = amqpConnectionFactory;
            this._logger = logger;
            this._amqpChannels = new ConcurrentDictionary<int, Channel>();
            this._amqpConfig = new ConnectionConfig
            {
                AutoRecovery = true,
                ConnectionName = $"Sensor:[{config.DeviceBusClient}].[{_config.SdrnDeviceSensorName}].[{_config.SdrnDeviceSensorTechId}].Publisher",
                HostName = _config.RabbitMqHost,
                VirtualHost = _config.RabbitMqVirtualHost,
                Port = _config.RabbitMqPort,
                UserName = _config.RabbitMqUser,
                Password = _config.RabbitMqPassword
            };
        }

        public void Dispose()
        {
            try
            {
                if (_amqpChannels.Count > 0)
                {
                    foreach (var channel in this._amqpChannels.Values)
                    {
                        channel.Dispose();
                    }
                    _amqpChannels.Clear();
                }
                if (_amqpConnection != null)
                {
                    _amqpConnection.Dispose();
                    _amqpConnection = null;
                }
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.Disposing", $"An error occurred while disposing object", e, this);
            }
            
        }

        private bool TryToEstablishConnection()
        {
            if (this._amqpConnection == null)
            {
                lock(_connectionLocker)
                {
                    if (this._amqpConnection == null)
                    {
                        try
                        {
                            var connection = this._amqpConnectionFactory.Create(_amqpConfig);
                            this._amqpConnection = connection;
                        }
                        catch(Exception e)
                        {
                            this._logger.Exception("DeviceBus.ConnectionEstablishment", $"An error occurred while establishing connection to Device Bus: {_amqpConfig}", e, this);
                            return false;
                        }

                        // need to declare server's queues
                        if (this._amqpConnection.IsOpen)
                        {
                            if (!this.TryGetChannelForCurrentThread(out var amqpChannel))
                            {
                                this._amqpConnection.Dispose();
                                this._amqpConnection = null;
                                return false;
                            }

                            if (!this.TryDeclareServerQueues((amqpChannel)))
                            {
                                this.Dispose();
                                return false;
                            }
                        }
                        else
                        {
                            this._amqpConnection.Dispose();
                            this._amqpConnection = null;
                            return false;
                        }
                    }
                        
                }
                
            }

            var state =  this._amqpConnection.IsOpen;
            return state;
        }

        private bool TryGetChannelForCurrentThread(out Channel channel )
        {
            var result = false;
            channel = null;
            try
            {
                var threadId = Thread.CurrentThread.ManagedThreadId;

                if (!_amqpChannels.TryGetValue(threadId, out channel))
                {
                    channel = _amqpConnection.CreateChannel();
                    channel.EstablishChannel();

                    if (!_amqpChannels.TryAdd(threadId, channel))
                    {
                        channel.Dispose();
                        channel = null;
                        _amqpChannels.TryGetValue(threadId, out channel);
                    }
                }
                else
                {
                    if (!channel.IsOpen)
                    {
                        channel.EstablishChannel();
                    }
                }

                if (channel != null) result = channel.IsOpen;
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.ChannelEnsuring", $"An error occurred while ensuring channel of the Device Bus: {_amqpConfig}", e, this);
                result = false;
            }
            return result;
        }

        private bool TryDeclareServerQueues(Channel channel)
        {
            var deviceExchangeName = this._config.BuildDeviceExchangeName();
            if (!this.TryDeclareExchange(channel, deviceExchangeName))
            {
               return false;
            }

            var bindings = this._config.MessagesBindings.Values;
            foreach (var binding in bindings)
            {
                var queue = new QueueDescriptor
                {
                    Exchange = deviceExchangeName,
                    Name = this._config.BuildServerQueueName(binding.RoutingKey),
                    RoutingKey = $"[{this._config.SdrnServerInstance}].[{binding.RoutingKey}]"
                };
                if (!this.TryDeclareQueue(channel, queue.Name, queue.RoutingKey, queue.Exchange))
                {
                    return false;
                }
            }

            var trashQueue = new QueueDescriptor
            {
                Exchange = deviceExchangeName,
                Name = this._config.BuildServerQueueName("trash"),
                RoutingKey = $"[{this._config.SdrnServerInstance}].[trash]"
            };
            if (!this.TryDeclareQueue(channel, trashQueue.Name, trashQueue.RoutingKey, trashQueue.Exchange))
            {
                return false;
            }

            var errorsQueue = new QueueDescriptor
            {
                Exchange = deviceExchangeName,
                Name = this._config.BuildServerQueueName("errors"),
                RoutingKey = $"[{this._config.SdrnServerInstance}].[errors]"
            };
            if (!this.TryDeclareQueue(channel, errorsQueue.Name, errorsQueue.RoutingKey, errorsQueue.Exchange))
            {
                return false;
            }

            return true;
        }

        private bool TryDeclareExchange(Channel channel, string exchangeName)
        {
            bool result;
            try
            {
                channel.DeclareDurableDirectExchange(exchangeName);
                result = true;
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.Configuration", $"An error occurred during the configuration of the Device Bus: Exchange = '{exchangeName}'", e, this);
                result = false;
            }
            return result;
        }

        private bool TryDeclareQueue(Channel channel, string queueName, string bindKey, string exchangeName)
        {
            bool result;
            try
            {
                channel.DeclareDurableQueue(queueName, exchangeName, bindKey);
                result = true;
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.Configuration", $"An error occurred during the configuration of the Device Bus: Queue = '{queueName}'", e, this);
                result = false;
            }
            return result;
        }

        private ExchangeDescriptor DefineExchangeDescriptor(string messageType)
        {
            var exchangeDescriptor = new ExchangeDescriptor
            {
                RoutingKey = this._config.GetRoutingKeyByMessageType(messageType),
                Exchange = this._config.BuildDeviceExchangeName()
            };

            return exchangeDescriptor;
        }

        public bool TryToSend(BusMessage message)
        {
            if (!this.TryToEstablishConnection())
            {
                this._logger.Error("DeviceBus.MessageSending", $"The connection to the Device Bus was not established: {_amqpConfig}", this);
                return false;
            }

            if (!this.TryGetChannelForCurrentThread(out var channel))
            {
                this._logger.Error("DeviceBus.MessageSending", $"The channel of the Device Bus was not established: {_amqpConfig}", this);
                return false;
            }

            var exchange = this.DefineExchangeDescriptor(message.Type);

            var amqpMessage = channel.CreateMessage();
            amqpMessage.Id = message.Id;
            amqpMessage.CorrelationId = message.CorrelationToken;
            amqpMessage.Type = message.Type;
            amqpMessage.AppId = message.Application;
            amqpMessage.ContentEncoding = message.BuildContentEncoding();
            amqpMessage.ContentType = "application/" + message.ContentType.ToString().ToLower();
            amqpMessage.Headers = new Dictionary<string, object>
            {
                [Protocol.Header.ApiVersion] = message.ApiVersion,
                [Protocol.Header.Protocol] = message.Protocol,
                [Protocol.Header.Created] = message.Created.ToString("o"),
                [Protocol.Header.SdrnServer] = message.Server,
                [Protocol.Header.SensorName] = message.Sensor,
                [Protocol.Header.SensorTechId] = message.TechId,
                [Protocol.Header.BodyAqName] = message.BodyAQName,
            };
            amqpMessage.Body = message.AsBytes();
            
            try
            {
                channel.Publish(exchange.Exchange, exchange.RoutingKey, amqpMessage);
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.MessageSending", $"An error occurred while publishing message to Device Bus: {message}", e, this);
                return false;
            }
            
            return true;
        }

    }
}
