using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Modules.Sdrn.AmqpBroker;
using Atdi.Modules.Sdrn.MessageBus;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MD = Atdi.DataModels.Sdrns.Server.Entities;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class SdrnQueueConsumer : IMessageHandler
    {
        private readonly string _tag;
        private readonly string _routingKey;
        private readonly string _queue;
        private readonly MessageConverter _messageConverter;
        private readonly SdrnHandlerLibrary _handlerLibrary;
        private readonly BusConnection _busConnection;
        private readonly SdrnBusControllerConfig _busControllerConfig;
        private readonly MessageProcessing _messageProcessing;
        private readonly IServicesResolver _servicesResolver;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IQueryBuilder<MD.IAmqpMessage> _amqpMessageQueryBuilder;
        private readonly IQueryBuilder<MD.IAmqpEvent> _amqpEventQueryBuilder;

        public SdrnQueueConsumer(
            string tag, 
            string routingKey, 
            string queue, 
            MessageConverter messageConverter, 
            SdrnHandlerLibrary handlerLibrary, 
            BusConnection busConnection, 
            SdrnBusControllerConfig busControllerConfig,
            MessageProcessing messageProcessing,
            IServicesResolver servicesResolver, 
            IDataLayer<EntityDataOrm> dataLayer,
            ILogger logger)
        {
            this._tag = tag;
            this._routingKey = routingKey;
            this._queue = queue;
            this._messageConverter = messageConverter;
            this._handlerLibrary = handlerLibrary;
            this._busConnection = busConnection;
            this._busControllerConfig = busControllerConfig;
            this._messageProcessing = messageProcessing;
            this._servicesResolver = servicesResolver;
            this._dataLayer = dataLayer;
            this._logger = logger;
            this._queryExecutor = this._dataLayer.Executor<SdrnServerDataContext>();
            this._amqpMessageQueryBuilder = this._dataLayer.GetBuilder<MD.IAmqpMessage>();
            this._amqpEventQueryBuilder = this._dataLayer.GetBuilder<MD.IAmqpEvent>();
        }

        public MessageHandlingResult Handle(Message message, IDeliveryContext deliveryContext)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.Processing, this))
            {

                _logger.Verbouse(Contexts.ThisComponent, Categories.Processing, Events.ReceivedMessage.With(deliveryContext.ConsumerTag, deliveryContext.RoutingKey, deliveryContext.Exchange, deliveryContext.DeliveryTag, message.Type, message.Id));

                var handlingResult = new SdrnMessageHandlingResult();

                try
                {
                    if (string.IsNullOrEmpty(message.Id) 
                        || string.IsNullOrEmpty(message.Type) 
                        || string.IsNullOrEmpty(message.AppId)
                        || string.IsNullOrEmpty(message.ContentType)
                        || message.Headers == null)
                    {
                        handlingResult.Status = SdrnMessageHandlingStatus.Trash;
                        handlingResult.ReasonFailure = "The message contains an incorrect head";
                        this.RedirectMessage(message, deliveryContext, handlingResult);
                        return MessageHandlingResult.Confirm;
                    }

                    if (!"application/sdrn".Equals(message.ContentType))
                    {
                        throw new InvalidOperationException("The message contains an incorrect conent type");
                    }

                    var sdrnServer = GetHeaderValue(message.Headers, "SdrnServer");
                    if (!_busControllerConfig.Environment.ServerInstance.Equals(sdrnServer, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("The message contains an incorrect SDRN server name");
                    }
                    var sensorName = GetHeaderValue(message.Headers, "SensorName");
                    if (string.IsNullOrWhiteSpace(sensorName))
                    {
                        throw new InvalidOperationException("The message contains an incorrect Sensor Name");
                    }
                    var sensorTechId = GetHeaderValue(message.Headers, "SensorTechId");
                    if (string.IsNullOrWhiteSpace(sensorName))
                    {
                        throw new InvalidOperationException("The message contains an incorrect Sensor Tech Id");
                    }


                    var queryInsert = this._amqpMessageQueryBuilder
                        .Insert()
                        .SetValue(c => c.StatusCode, (byte)0) // 0 - Created, 1 - Event Send, 2 - start processing, 3 - finish processed
                        .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                        .SetValue(c => c.ThreadId, System.Threading.Thread.CurrentThread.ManagedThreadId)
                        .SetValue(c => c.PropRoutingKey, deliveryContext.RoutingKey)
                        .SetValue(c => c.PropExchange, deliveryContext.Exchange)
                        .SetValue(c => c.PropDeliveryTag, deliveryContext.DeliveryTag)
                        .SetValue(c => c.PropConsumerTag, deliveryContext.ConsumerTag)

                        .SetValue(c => c.PropAppId, message.AppId)
                        .SetValue(c => c.PropMessageId, message.Id)
                        .SetValue(c => c.PropType, message.Type)
                        .SetValue(c => c.PropContentEncoding, message.ContentEncoding)
                        .SetValue(c => c.PropContentType, message.ContentType)
                        .SetValue(c => c.PropCorrelationId, message.CorrelationId)
                        .SetValue(c => c.PropTimestamp, message.Timestamp)
                        .SetValue(c => c.HeaderCreated, GetHeaderValue(message.Headers, "Created"))
                        .SetValue(c => c.HeaderSdrnServer, sdrnServer)
                        .SetValue(c => c.HeaderSensorName, sensorName)
                        .SetValue(c => c.HeaderSensorTechId, sensorTechId)

                        .SetValue(c => c.BodyContentType, "json")
                        .SetValue(c => c.BodyContentEncoding, "compressed")
                        .SetValue(c => c.BodyContent, Compressor.Compress(message.Body))
                        .Select(c => c.Id);


                    var messageId_PK = _queryExecutor.Execute<MD.IAmqpMessage_PK>(queryInsert);


                    var eventQueryInsert = this._amqpEventQueryBuilder
                        .Insert()
                        .SetValue(c => c.Id, messageId_PK.Id)
                        .SetValue(c => c.PropType, message.Type);


                    _queryExecutor.Execute<MD.IAmqpEvent_PK>(eventQueryInsert);

                    _messageProcessing.OnCreatedMessage();

                    //var busEvent = new DevicesBusEvent($"On{message.Type}DeviceBusEvent", "SdrnDeviceBusController")
                    //{
                    //    BusMessageId = messageId
                    //};

                    //_eventEmitter.Emit(busEvent);

                    return MessageHandlingResult.Confirm;
                    /*
                    var messageObject = _messageConverter.Deserialize(message);
                    var handlerTypes = this._handlerLibrary.GetHandlerTypes(message.Type, messageObject.Type);


                    if (handlerTypes.Length == 0)
                    {
                        this.RedirectMessage(message, deliveryContext, handlingResult);
                        return MessageHandlingResult.Confirm;
                    }

                    

                    var messageHeader = new SdrnIncomingEnvelopeProperties
                    {
                        MessageId = message.Id,
                        MessageType = message.Type,
                        CorrelationToken = message.CorrelationId,
                        Created = Convert.ToDateTime(GetHeaderValue(message.Headers, "Created")),
                        SensorName = GetHeaderValue(message.Headers, "SensorName"),
                        SensorTechId = GetHeaderValue(message.Headers, "SensorTechId"),
                    };

                    var envelopeType = typeof(SdrnIncomingEnvelope<>);
                    var envelopeGenericType = envelopeType.MakeGenericType(messageObject.Type);
                    var incomingEnvelope = Activator.CreateInstance(envelopeGenericType, messageHeader, messageObject.Object);

                    for (int i = 0; i < handlerTypes.Length; i++)
                    {
                        var handlerType = handlerTypes[i];

                        // void Handle(ISdrnIncomingEnvelope<TDeliveryObject> incomingEnvelope, ISdrnMessageHandlingResult result);


                        var handleMethod = handlerType.GetMethod("Handle");
                        var handlerInstance = _handlerLibrary.ResolveHandler(handlerType);
                        handleMethod.Invoke(handlerInstance, new object[] { incomingEnvelope, handlingResult });

                        if (handlingResult.Status == SdrnMessageHandlingStatus.Confirmed)
                        {
                            return MessageHandlingResult.Confirm;
                        }
                        if (handlingResult.Status == SdrnMessageHandlingStatus.Ignored)
                        {
                            return MessageHandlingResult.Ignore;
                        }
                        if (handlingResult.Status == SdrnMessageHandlingStatus.Error)
                        {
                            this.RedirectMessage(message, deliveryContext, handlingResult);
                            return MessageHandlingResult.Confirm;
                        }
                        if (handlingResult.Status == SdrnMessageHandlingStatus.Rejected)
                        {
                            this.RedirectMessage(message, deliveryContext, handlingResult);
                            return MessageHandlingResult.Reject;
                        }
                        if (handlingResult.Status == SdrnMessageHandlingStatus.Trash)
                        {
                            this.RedirectMessage(message, deliveryContext, handlingResult);
                            return MessageHandlingResult.Confirm;
                        }
                    }

                    this.RedirectMessage(message, deliveryContext, handlingResult);
                    return MessageHandlingResult.Confirm;
                    */
                }
                catch (Exception e)
                {
                    _logger.Exception(Contexts.ThisComponent, Categories.Processing, e);

                    handlingResult.Status = SdrnMessageHandlingStatus.Error;
                    handlingResult.ReasonFailure = e.Message;
                    this.RedirectMessage(message, deliveryContext, handlingResult);
                    return MessageHandlingResult.Confirm;
                }
            }
        }

        private string GetHeaderValue(IDictionary<string, object> headers, string key)
        {
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

        private void RedirectMessage(Message message, IDeliveryContext deliveryContext, ISdrnMessageHandlingResult handlingResult)
        {
            var routingKey = string.Empty;
            if (handlingResult.Status == SdrnMessageHandlingStatus.Error)
            {
                routingKey = _busControllerConfig.BuildServerErrorQueueRoute(this._routingKey);
            }
            else if (handlingResult.Status == SdrnMessageHandlingStatus.Rejected)
            {
                routingKey = _busControllerConfig.BuildServerRejectedQueueRoute(this._routingKey);
            }
            else if (handlingResult.Status == SdrnMessageHandlingStatus.Trash)
            {
                routingKey = _busControllerConfig.BuildServerTrashQueueRoute(this._routingKey);
            }
            else if (handlingResult.Status == SdrnMessageHandlingStatus.Unprocessed)
            {
                routingKey = _busControllerConfig.BuildServerUnprocessedQueueRoute(this._routingKey);
            }
            else
            {
                return;
            }

            var redirectedMessage = new Message
            {
                Id = Guid.NewGuid().ToString(),
                Type = "RedirectMessage",
                CorrelationId = message.Id,
                ContentType = message.ContentType,
                ContentEncoding = message.ContentEncoding,
                Body = message.Body,
                AppId = "Atdi.AppUnits.Sdrn.BusController.dll"
            };

            redirectedMessage.Headers = new Dictionary<string, object>
            {
                ["SdrnServer"] = this._busControllerConfig.Environment.ServerInstance,
                ["Created"] = DateTime.Now.ToString("o"),

                ["Message.Id"] = message.Id,
                ["Message.Type"] = message.Type,
                ["Message.AppId"] = message.AppId,
                ["Message.CorrelationId"] = message.CorrelationId,
                ["Message.ContentType"] = message.ContentType,
                ["Message.ContentEncoding"] = message.ContentEncoding,
                ["Message.SdrnServer"] = message.Headers["SdrnServer"],
                ["Message.SensorName"] = message.Headers["SensorName"],
                ["Message.SensorTechId"] = message.Headers["SensorTechId"],
                ["Message.Created"] = message.Headers["Created"],
                
                ["Processing.Status"] = handlingResult.Status.ToString(),
                ["Processing.ReasonFailure"] = handlingResult.ReasonFailure
            };

            //var factory = _servicesResolver.Resolve<BusConnectionFactory>();
            //var busConfig = new BusConnectionConfig
            //{
            //    ApplicationName = _busControllerConfig.Environment.GetAppName(),
            //    ConnectionName = $"[SDRN.Server].[{_busControllerConfig.Environment.ServerInstance}].[Consumer].[{this._tag}].[#{System.Threading.Thread.CurrentThread.ManagedThreadId}]",
            //    AutoRecovery = true,
            //    HostName = _busControllerConfig.BusHost,
            //    VirtualHost = _busControllerConfig.BusVirtualHost,
            //    Port = _busControllerConfig.BusPort,
            //    UserName = _busControllerConfig.BusUser,
            //    Password = _busControllerConfig.BusPassword
            //};

            //using (var busConnection = factory.Create(busConfig))
            {
                _busConnection.Publish(_busControllerConfig.GetServerInnerExchangeName(), routingKey, redirectedMessage);
            }
        }

        public void Join()
        {
            this._busConnection.JoinConsumer(_queue, _tag, this);
        }

        public void Unjoin()
        {
            this._busConnection.UnjoinConsumer(_queue, _tag, this);
        }
    }
}
