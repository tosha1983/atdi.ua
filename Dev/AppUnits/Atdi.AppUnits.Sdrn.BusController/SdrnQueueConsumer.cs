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
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.Modules.Sdrn.DeviceBus;
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
		private readonly IQueryBuilder<MD.IAmqpMessageLog> _amqpMessageLogQueryBuilder;
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
			this._amqpMessageLogQueryBuilder = this._dataLayer.GetBuilder<MD.IAmqpMessageLog>();
			this._amqpEventQueryBuilder = this._dataLayer.GetBuilder<MD.IAmqpEvent>();
        }

        public MessageHandlingResult Handle(Message message, IDeliveryContext deliveryContext)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.Processing, this))
            {

                _logger.Verbouse(Contexts.ThisComponent, Categories.Processing, Events.ReceivedMessage.With(deliveryContext.ConsumerTag, deliveryContext.RoutingKey, deliveryContext.Exchange, deliveryContext.DeliveryTag, message.Type, message.Id));

                try
                {
                    if (string.IsNullOrEmpty(message.Id)
                        || string.IsNullOrEmpty(message.Type)
                        || string.IsNullOrEmpty(message.AppId)
                        || string.IsNullOrEmpty(message.ContentType)
                        || message.Headers == null)
                    {
                        var handlingResult = new SdrnMessageHandlingResult
                        {
                            Status = SdrnMessageHandlingStatus.Trash,
                            ReasonFailure = "The message contains an incorrect header"
                        };
                        this.RedirectMessage(message, deliveryContext, handlingResult);
                        return MessageHandlingResult.Confirm;
                    }

                    if (!Protocol.ContentType.Check(message.ContentType))
                    {
                        throw new InvalidOperationException("The message contains an incorrect Content Type");
                    }

                    var sdrnServer = GetHeaderValue(message.Headers, Protocol.Header.SdrnServer);
                    if (!_busControllerConfig.Environment.ServerInstance.Equals(sdrnServer,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("The message contains an incorrect SDRN Server Name");
                    }

                    var sensorName = GetHeaderValue(message.Headers, Protocol.Header.SensorName);
                    if (string.IsNullOrWhiteSpace(sensorName))
                    {
                        throw new InvalidOperationException("The message contains an incorrect Sensor Name");
                    }

                    var sensorTechId = GetHeaderValue(message.Headers, Protocol.Header.SensorTechId);
                    if (string.IsNullOrWhiteSpace(sensorName))
                    {
                        throw new InvalidOperationException("The message contains an incorrect Sensor Tech Id");
                    }

                    var headerApiVersion = GetHeaderValue(message.Headers, Protocol.Header.ApiVersion);
                    var headerProtocol = GetHeaderValue(message.Headers, Protocol.Header.Protocol);
                    var headerBodyAqName = GetHeaderValue(message.Headers, Protocol.Header.BodyAqName);
                    var headerCreated = GetHeaderValue(message.Headers, Protocol.Header.Created);

                    var currentThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

					var queryInsert = this._amqpMessageQueryBuilder
                        .Insert()
                        .SetValue(c => c.StatusCode,
                            (byte) 0) // 0 - Created, 1 - Send Event , 2 - start processing, 3 - finish processed
						.SetValue(c => c.StatusName, "Created") //MessageProcessingStatus.Created.ToString())
                        .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                        .SetValue(c => c.ThreadId, currentThreadId)
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

                        .SetValue(c => c.HeaderCreated, headerCreated)
                        .SetValue(c => c.HeaderSdrnServer, sdrnServer)
                        .SetValue(c => c.HeaderSensorName, sensorName)
                        .SetValue(c => c.HeaderSensorTechId, sensorTechId)
                        .SetValue(c => c.HeaderApiVersion, headerApiVersion)
                        .SetValue(c => c.HeaderProtocol, headerProtocol)
                        .SetValue(c => c.HeaderBodyAQName, headerBodyAqName)

                        .SetValue(c => c.BodyContentType, Protocol.ContentType.QualifiedOriginal)
                        .SetValue(c => c.BodyContentEncoding, Protocol.ContentEncoding.Compressed)
                        .SetValue(c => c.BodyContent, Compressor.Compress(message.Body));

                    var messagePk = _queryExecutor.Execute<MD.IAmqpMessage_PK>(queryInsert);

                    var logQueryInsert = this._amqpMessageLogQueryBuilder
	                    .Insert()
	                    .SetValue(c => c.MESSAGE.Id, messagePk.Id)
						.SetValue(c => c.StatusCode, (byte)MD.AmqpMessageStatusCode.Created)
	                    .SetValue(c => c.StatusName, "Created")
	                    .SetValue(c => c.StatusNote, "Message was created in DB")
						.SetValue(c => c.CreatedDate, DateTimeOffset.Now)
	                    .SetValue(c => c.ThreadId, currentThreadId)
						.SetValue(c => c.Source, $"SDRN.QueueConsumer:[Tag={deliveryContext.ConsumerTag}][Handle]");
                    _queryExecutor.Execute(logQueryInsert);

					var eventQueryInsert = this._amqpEventQueryBuilder
                        .Insert()
                        .SetValue(c => c.Id, messagePk.Id)
                        .SetValue(c => c.PropType, message.Type);
					_queryExecutor.Execute(eventQueryInsert);

                    _messageProcessing.OnCreatedMessage();

                }
                catch (Exception e)
                {
                    _logger.Exception(Contexts.ThisComponent, Categories.Processing, e);
                    var handlingResult = new SdrnMessageHandlingResult
                    {
                        Status = SdrnMessageHandlingStatus.Error,
                        ReasonFailure = e.Message
                    };
                    this.RedirectMessage(message, deliveryContext, handlingResult);
                }

                return MessageHandlingResult.Confirm;
            }
        }

        private static string GetHeaderValue(IDictionary<string, object> headers, string key)
        {
            if (headers == null)
            {
                return null;
            }
            if (headers.TryGetValue(key, out var value))
            {
                return Convert.ToString(Encoding.UTF8.GetString(((byte[])value)));
            }
            return null;
        }

        private void RedirectMessage(Message message, IDeliveryContext deliveryContext, ISdrnMessageHandlingResult handlingResult)
        {
            var routingKey = string.Empty;
            try
            {
                
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
                    Id = message.Id,
                    Type = message.Type,
                    CorrelationId = message.Id,
                    ContentType = message.ContentType,
                    ContentEncoding = message.ContentEncoding,
                    Body = message.Body,
                    AppId = message.AppId,
                    Timestamp = message.Timestamp
                };
                if (message.Headers != null && message.Headers.Count > 0)
                {
                    try
                    {
                        redirectedMessage.Headers = new Dictionary<string, object>(message.Headers);
                    }
                    catch (Exception e)
                    {
                        _logger.Exception(Contexts.ThisComponent, Categories.Processing, e, this);
                        redirectedMessage.Headers = new Dictionary<string, object>();
                    }
                    
                }
                else
                {
                    redirectedMessage.Headers = new Dictionary<string, object>();
                }

                redirectedMessage.Headers["Redirect.Created"] = DateTime.Now.ToString("o");
                redirectedMessage.Headers["Redirect.Status"] = handlingResult.Status.ToString();
                redirectedMessage.Headers["Redirect.Reason"] = handlingResult.ReasonFailure;

                _busConnection.Publish(_busControllerConfig.GetServerInnerExchangeName(), routingKey, redirectedMessage);
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Processing, e, this);
                _logger.Error(Contexts.ThisComponent, Categories.Processing, $"Failed to redirect message: Reason='{handlingResult?.ReasonFailure}', Status='{handlingResult?.Status.ToString()}', RoutingKey='{routingKey}', Message='{message?.Id}', Type='{message?.Type}', AppId='{message?.AppId}', ContentType='{message?.ContentType}'");
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
