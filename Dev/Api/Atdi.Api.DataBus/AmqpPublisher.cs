using Atdi.DataModels.Api.DataBus;
using Atdi.Modules.AmqpBroker;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Atdi.Api.DataBus
{
    internal sealed class AmqpPublisher : IDisposable
    {
        private object _connectionLocker = new object();

        private readonly ConnectionConfig _amqpConfig;
        private readonly IBusConfig _config;
        private readonly ConnectionFactory _amqpConnectionFactory;
        private readonly BusLogger _logger;
        private Connection _amqpConnection;
        private readonly ConcurrentDictionary<int, Channel> _amqpChannels;

        public AmqpPublisher(IBusConfig config, ConnectionFactory amqpConnectionFactory, BusLogger logger)
        {
            this._config = config;
            this._amqpConnectionFactory = amqpConnectionFactory;
            this._logger = logger;
            this._amqpChannels = new ConcurrentDictionary<int, Channel>();
            this._amqpConfig = new ConnectionConfig
            {
                AutoRecovery = true,
                ConnectionName = $"DataBus.[{_config.Name}].[{_config.Address}].[v{_config.ApiVersion}].Publisher",
                HostName = _config.Host,
                VirtualHost = _config.VirtualHost,
                Port = _config.Port,
                UserName = _config.User,
                Password = _config.Password
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
                this._logger.Exception(BusContexts.Disposing, $"An error occurred while disposing object", e, this);
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
                            this._logger.Exception(BusContexts.ConnectionEstablishing, $"An error occurred while establishing connection to Data Bus: {_amqpConfig}", e, this);
                            return false;
                        }
                    }
                        
                }
                
            }

            var state =  this._amqpConnection.IsOpen;
            return state;
        }

        private bool TryToEstablishChannel(out Channel channel )
        {
            var result = false;
            channel = null;
            try
            {
                var threadId = Thread.CurrentThread.ManagedThreadId;

                if (!_amqpChannels.TryGetValue(threadId, out channel))
                {
                    channel = _amqpConnection.CreateChannel();
                    if (!_amqpChannels.TryAdd(threadId, channel))
                    {
                        channel.Dispose();
                        channel = null;
                        _amqpChannels.TryGetValue(threadId, out channel);
                    }
                    else
                    {
                        channel.EstablishChannel();
                    }
                }
                result = channel.IsOpen;
            }
            catch (Exception e)
            {
                this._logger.Exception(BusContexts.ConnectionEstablishing, $"An error occurred while establishing channel to Data Bus: {_amqpConfig}", e, this);
                result = false;
            }
            return result;
        }

        private bool TryDeclareCommonExchenge(Channel channel, string exchegeName)
        {
            var result = false;
            try
            {
                channel.DeclareDurableDirectExchange(exchegeName);
                result = true;
            }
            catch (Exception e)
            {
                this._logger.Exception(BusContexts.ConnectionEstablishing, $"An error occurred during the configuration of the Data Bus: Declaring Exchange = '{exchegeName}'", e, this);
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

        public bool TryToSend(Message message)
        {
            if (!this.TryToEstablishConnection())
            {
                this._logger.Error(0, BusContexts.MessageSending, $"The connection to the Data Hub is not established: {_amqpConfig}", this);
                return false;
            }

            if (!this.TryToEstablishChannel(out Channel channel))
            {
                this._logger.Error(0, BusContexts.MessageSending, $"The channel of the Data Hub is not established: {_amqpConfig}", this);
                return false;
            }

            var exchangeName = $"E.DataBus.[{_config.Name}].Common.[v{_config.ApiVersion}]";

            if (!this.TryDeclareCommonExchenge(channel, exchangeName))
            {
                this._logger.Error(0, BusContexts.MessageSending, $"The common exchenge of the Data Hub is not established: {_amqpConfig}", this);
                return false;
            }

            var queueName = BuildQueueName(message.QueueType, message.Type, message.To, _config.Name, message.SpecificQueue, _config.ApiVersion);

            //switch (message.QueueType)
            //{
            //    case QueueType.Common:
            //        queueName = $"Q.DataBus.[{_config.Name}].[{message.To}].Common.[v{_config.ApiVersion}]";
            //        break;
            //    case QueueType.Private:
            //        queueName = $"Q.DataBus.[{_config.Name}].[{message.To}].[{message.Type}].[v{_config.ApiVersion}]";
            //        break;
            //    case QueueType.Specific:
            //        queueName = $"Q.DataBus.[{_config.Name}].[{message.To}].[{message.SpecificQueue}].[v{_config.ApiVersion}]";
            //        break;
            //    default:
            //        queueName = $"Q.DataBus.[{_config.Name}].[{message.To}].Common.[v{_config.ApiVersion}]";
            //        break;
            //}

            var routingKey = $"RK.[{message.To}].[{message.Type}]";

            if (!this.TryDeclareQueue(channel, queueName, routingKey, exchangeName))
            {
                this._logger.Error(0, BusContexts.MessageSending, $"The queue of the Data Hub is not established: {_amqpConfig}", this);
                return false;
            }

            var amqpMessage = channel.CreateMessage();
            amqpMessage.Id = message.Id;
            amqpMessage.Type = message.Type;
            amqpMessage.AppId = message.Application;
            amqpMessage.ContentEncoding = message.BuildContentEncoding();
            amqpMessage.ContentType = "application/" + message.ContentType;
            amqpMessage.Headers = new Dictionary<string, object>
            {
                ["ApiVersion"] = message.ApiVersion,
                ["BusName"] = message.BusName,
                ["Created"] = message.Created.ToString("o"),
                ["To"] = message.To,
                ["From"] = message.From,
                ["MessageTypeAQName"] = message.MessageTypeAQName,
                ["DeliveryObjectAQName"] = message.DeliveryObjectAQName,
            };
            switch (message.ContentType)
            {
                case ContentType.Json:
                    amqpMessage.Body = Encoding.UTF8.GetBytes((string)message.Body);
                    break;
                case ContentType.Xml:
                    amqpMessage.Body = Encoding.UTF8.GetBytes((string)message.Body);
                    break;
                case ContentType.Binary:
                    amqpMessage.Body = (byte[])message.Body;
                    break;
                default:
                    break;
            }
            
            try
            {
                channel.Publish(exchangeName, routingKey, amqpMessage);
            }
            catch (Exception e)
            {
                this._logger.Exception(BusContexts.MessageSending, $"An error occurred while publishing message to Data Bus: {message}", e, this);
                return false;
            }
            
            return true;
        }

        public static string BuildQueueName(QueueType queueType, string messageType, string address, string busName, string specificQueue, string apiVersion)
        {
            var queueName = "";
            switch (queueType)
            {
                case QueueType.Common:
                    queueName = $"Q.DataBus.[{busName}].[{address}].Common.[v{apiVersion}]";
                    break;
                case QueueType.Private:
                    queueName = $"Q.DataBus.[{busName}].[{address}].Private.[{messageType}].[v{apiVersion}]";
                    break;
                case QueueType.Specific:
                    queueName = $"Q.DataBus.[{busName}].[{address}].Specific.[{specificQueue}].[v{apiVersion}]";
                    break;
                default:
                    queueName = $"Q.DataBus.[{busName}].[{address}].Common.[v{apiVersion}]";
                    break;
            }
            return queueName;
        }
    }
}
