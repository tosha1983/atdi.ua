using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Modules.Sdrn.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal class MessageDispatcher : IMessageDispatcher
    {
        private readonly string _name;
        private readonly BusLogger _logger;
        private readonly EnvironmentDescriptor _environmentDescriptor;
        private readonly MessageConverter _messageConverter;
        private readonly RabbitMQBus _rabbitBus;
        private readonly Dictionary<string, List<MessageHandlerDescriptor>> _handlers;
        private MessageDispatcherState _state;
        private QueueConsumer _consumer;

        internal MessageDispatcher(string name, EnvironmentDescriptor environmentDescriptor, MessageConverter messageConverter, BusLogger logger)
        {
            this._name = name;
            this._logger = logger;
            this._environmentDescriptor = environmentDescriptor;
            this._messageConverter = messageConverter;
            this._rabbitBus = new RabbitMQBus("Dispatcher", environmentDescriptor, logger);
            this._state = MessageDispatcherState.Deactivated;
            this._handlers = new Dictionary<string, List<MessageHandlerDescriptor>>();
            this._consumer = this._rabbitBus.CreateConsumer(this._environmentDescriptor.BuildDeviceQueueName(), name, this);
        }

        public MessageDispatcherState State => this._state;

        public void Activate()
        {
            if (this._state == MessageDispatcherState.Activated)
            {
                return;
            }
            this._consumer.Activate();
            this._state = MessageDispatcherState.Activated;
        }

        public void Deactivate()
        {
            if (this._state == MessageDispatcherState.Deactivated)
            {
                return;
            }
            this._consumer.Deactivate();
            this._state = MessageDispatcherState.Deactivated;
        }

        public void Dispose()
        {
            if (this._state == MessageDispatcherState.Activated)
            {
                this.Deactivate();
            }

            this._rabbitBus.Dispose();
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

            var handlerDecriptor = new MessageHandlerDescriptor(handler, handler.MessageType);

            if (!_handlers.ContainsKey(handlerDecriptor.MessageType))
            {
                var list = new List<MessageHandlerDescriptor>
                {
                    handlerDecriptor
                };
                _handlers[handlerDecriptor.MessageType] = list;
            }
            else
            {
                var list = _handlers[handlerDecriptor.MessageType];
                list.Add(handlerDecriptor);
            }
        }

        public IMessageResult OnMessage(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            var msgSdrnServer = GetHeaderValue(properties, "SdrnServer");
            var msgSensorName = GetHeaderValue(properties, "SensorName");
            var msgSensorTechId = GetHeaderValue(properties, "SensorTechId");

            if (!_environmentDescriptor.SdrnServerInstance.Equals(msgSdrnServer, StringComparison.OrdinalIgnoreCase)
                || !_environmentDescriptor.SdrnDeviceSensorName.Equals(msgSensorName, StringComparison.OrdinalIgnoreCase)
                || !_environmentDescriptor.SdrnDeviceSensorTechId.Equals(msgSensorTechId, StringComparison.OrdinalIgnoreCase))
            {
                return new MessageResult
                {
                    Result = MessageHandlingResult.Error,
                    ReasonFailure = "Incorrect header data"
                };
            }

            if (!properties.IsTypePresent() || !properties.IsMessageIdPresent() || !properties.IsAppIdPresent())
            {
                return new MessageResult
                {
                    Result = MessageHandlingResult.Error,
                    ReasonFailure = "Incorrect message properties"
                };
            }

            var message = new Message
            {
                Id = properties.MessageId,
                Type = properties.Type,
                ContentType = properties.ContentType,
                ContentEncoding = properties.ContentEncoding,
                CorrelationId = properties.CorrelationId,
                Headers = properties.Headers,
                Body = body
            };

            if (!this._handlers.ContainsKey(message.Type))
            {
                return new MessageResult
                {
                    Result = MessageHandlingResult.Error,
                    ReasonFailure = "Unsupported message type"
                };
            }

            var messageToken = new MessageToken(message.Id, message.Type);
            var messageObject = this._messageConverter.Deserialize(message);
            var handlers = this._handlers[message.Type];

            foreach (var handler in handlers)
            {
                var receivedMessageParam = Activator.CreateInstance(
                    handler.ReceivedMessageGenericType,
                    new object[] 
                    {
                        messageToken,
                        messageObject.Object,
                        DateTime.Now,
                        message.CorrelationId
                    });

                try
                {
                    handler.OnHandleMethod.Invoke(handler.Instance, new object[] { receivedMessageParam });
                }
                catch(Exception e)
                {
                    _logger.Exception(BusEvents.ExceptionEvent, "DispatchMessage", e, this);

                    return new MessageResult
                    {
                        Result = MessageHandlingResult.Error,
                        ReasonFailure = "Error occurred while processing the message: " + e.Message
                    };
                }

                var result = receivedMessageParam as IMessageResult;

                if (result.Result == MessageHandlingResult.Received)
                {
                    continue;
                }
                if (result.Result == MessageHandlingResult.Ignore)
                {
                    continue;
                }

                return result;
            }

            return new MessageResult
            {
                Result = MessageHandlingResult.Received
            };
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

        private string GetHeaderValue(IBasicProperties properties, string key)
        {
            var headers = properties.Headers;
            if (headers == null)
            {
                return null;
            }
            if (!headers.ContainsKey(key))
            {
                return null;
            }
            return Convert.ToString(Encoding.UTF8.GetString(((byte[])headers[key])));
        }
    }
}
