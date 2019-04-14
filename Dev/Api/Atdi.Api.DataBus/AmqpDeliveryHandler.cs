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
    internal sealed class AmqpDeliveryHandler : IDeliveryHandler, IDisposable
    {
        private readonly string _tag;
        private readonly IBusConfig _config;
        private readonly string _queueName;
        private readonly HandlersHost _handlersHost;
        private readonly MessagePacker _messagePacker;
        private readonly IMessageHandlerResolver _handlerResolver;
        private readonly BusLogger _logger;
        private Channel _redirectChannel;
        private Channel _channel;

        /// <summary>
        /// Таймаут между неудачными попытками редиректа
        /// </summary>
        private int _redirectionTimeout = 30 * 1000;

        public AmqpDeliveryHandler(IBusConfig config, string queueName, HandlersHost handlersHost, IMessageHandlerResolver handlerResolver, MessagePacker messagePacker, BusLogger logger)
        {
            this._tag = queueName + "_" + this.GetHashCode().ToString();
            this._config = config;
            this._queueName = queueName;
            this._handlersHost = handlersHost;
            this._handlerResolver = handlerResolver;
            this._messagePacker = messagePacker;
            this._logger = logger;
        }

        public void Dispose()
        {
            this.UnJoin();
        }

        public HandlingResult Handle(IDeliveryMessage message, IDeliveryContext deliveryContext)
        {
            this._logger.Verbouse(BusContexts.MessageSending, $"The message is handling: Id = '{message.Id}', Type = '{message.Type}', Tag ='{this._tag}'", this);

            try
            {
                var errors = new StringBuilder();
                if (string.IsNullOrEmpty(message.Type))
                {
                    errors.AppendLine("Undefined Message Type");
                }

                if (string.IsNullOrEmpty(message.ContentType))
                {
                    errors.AppendLine("Undefined Content Type");
                }

                var apiVersion = message.GetHeaderValue("ApiVersion");
                if (string.IsNullOrEmpty(apiVersion))
                {
                    errors.AppendLine("Undefined Api Version");
                }
                else if (!apiVersion.Equals(_config.ApiVersion, StringComparison.OrdinalIgnoreCase))
                {
                    errors.AppendLine($"Incorrect Api Version '{apiVersion}', expected '{_config.ApiVersion}'");
                }

                var busName = message.GetHeaderValue("BusName");
                if (string.IsNullOrEmpty(busName))
                {
                    errors.AppendLine("Undefined Bus Name");
                }
                else if (!busName.Equals(_config.Name, StringComparison.OrdinalIgnoreCase))
                {
                    errors.AppendLine($"Incorrect Bus Name '{busName}', expected '{_config.Name}'");
                }

                var addressTo = message.GetHeaderValue("To");
                if (string.IsNullOrEmpty(addressTo))
                {
                    errors.AppendLine("Undefined address To");
                }
                else if (!addressTo.Equals(_config.Address, StringComparison.OrdinalIgnoreCase))
                {
                    errors.AppendLine($"Incorrect address To '{addressTo}', expected '{_config.Address}'");
                }

                var addressFrom = message.GetHeaderValue("From");
                if (string.IsNullOrEmpty(addressFrom))
                {
                    errors.AppendLine("Undefined address From");
                }

                var messageTypeAQName = message.GetHeaderValue("MessageTypeAQName");
                if (string.IsNullOrEmpty(messageTypeAQName))
                {
                    errors.AppendLine("Undefined Message Type AQ Name");
                }
                var messageTypeType = Type.GetType(messageTypeAQName);
                if (messageTypeType == null)
                {
                    errors.AppendLine($"Incorrect Message Type AQ Name '{messageTypeAQName}'");
                }

                var deliveryObjectAQName = message.GetHeaderValue("DeliveryObjectAQName");
                if (string.IsNullOrEmpty(deliveryObjectAQName))
                {
                    errors.AppendLine("Undefined Delivery Object AQ Name");
                }
                var deliveryObjectType = Type.GetType(deliveryObjectAQName);
                if (deliveryObjectType == null)
                {
                    errors.AppendLine($"Incorrect Delivery Object AQ Name '{deliveryObjectAQName}'");
                }

                if (errors.Length > 0)
                {
                    throw new InvalidOperationException($"Incorrect message header: {Environment.NewLine + errors.ToString()}");
                }

                var deliveryObject = this.UnpuckDeliveryObject(message.ContentType, message.ContentEncoding, message.Body, deliveryObjectType);
                var token = new MessageToken(message.Id, message.Type);
                var created = DateTimeOffset.Now;
                var createdValue = message.GetHeaderValue("Created");
                if (!string.IsNullOrEmpty(createdValue))
                {
                    DateTimeOffset.TryParse(createdValue, out created);
                }
                var envelop = this.CreateIncomeEnvelopInstance(messageTypeType, deliveryObjectType, deliveryObject, addressFrom, token, created);
                //envelop.From = addressFrom;

                var handlers = this._handlersHost.GetHandlers(messageTypeType, deliveryObjectType).ToArray();
                var handlingResult = new HandlingResultData
                {
                    Status = MessageHandlingStatus.Unprocessed
                };

                for (int i = 0; i < handlers.Length; i++)
                {
                    var handler = handlers[i];
                    var handlerInstance = _handlerResolver.Resolve(handler.HandlerType);

                    try
                    {
                        handler.Invoker(handlerInstance, envelop, handlingResult);
                        if (handlingResult.Status == MessageHandlingStatus.Confirmed)
                        {
                            break;
                        }
                        if (handlingResult.Status == MessageHandlingStatus.Rejected)
                        {
                            break;
                        }
                    }
                    catch(Exception e)
                    {
                        this._logger.Exception(BusContexts.MessageSending, $"An error occurred while handle message by handler: HandlerType = '{handler.HandlerType.FullName}', Tag ='{this._tag}'", e, this);
                        handlingResult.Status = MessageHandlingStatus.Rejected;
                        handlingResult.Reason = $"An error occurred while handle message by handler: HandlerType = '{handler.HandlerType.FullName}', Error ='{e.Message}'";
                        handlingResult.Detail = e.ToString();
                        break;
                    }
                }

                if (handlingResult.Status == MessageHandlingStatus.Rejected)
                {
                    this.RedirectRejectedMessage(message, deliveryContext, handlingResult.Reason, handlingResult.Detail);
                }
                if (handlingResult.Status == MessageHandlingStatus.Unprocessed)
                {
                    this.RedirectUnprocessedMessage(message, deliveryContext, handlingResult.Reason, handlingResult.Detail);
                }

            }
            catch (Exception e)
            {
                this._logger.Exception(BusContexts.MessageSending, $"An error occurred while handling message: Tag ='{this._tag}'", e, this);
                this.RedirectRejectedMessage(message, deliveryContext, $"Exception: {e.Message}", e.ToString());
            }

            this._logger.Verbouse(BusContexts.MessageSending, $"The message was handled: Id = '{message.Id}', Type = '{message.Type}', Tag ='{this._tag}'", this);

            // основной принцип: мы все сообщения забираем с очереди
            return HandlingResult.Confirm;
        }

        private object UnpuckDeliveryObject(string contentTypeString, string contentEncodingString, byte[] body, Type deliveryObjectType)
        {
            var contentType = ContentType.Binary;
            var realBody = default(object);

            if ("application/json".Equals(contentTypeString, StringComparison.OrdinalIgnoreCase))
            {
                contentType = ContentType.Json;
                realBody = Encoding.UTF8.GetString(body);
            }
            else if ("application/xml".Equals(contentTypeString, StringComparison.OrdinalIgnoreCase))
            {
                contentType = ContentType.Xml;
                realBody = Encoding.UTF8.GetString(body);
            }
            else if ("application/binary".Equals(contentTypeString, StringComparison.OrdinalIgnoreCase))
            {
                contentType = ContentType.Binary;
                realBody = body;
            }


            var useCompressed = false;
            var useEncrypted = false;
            if (!string.IsNullOrEmpty(contentEncodingString))
            {
                useCompressed = contentEncodingString.Contains("compressed");
                useEncrypted = contentEncodingString.Contains("encrypted");
            }

            var convertor = _messagePacker.GetConvertor(contentType);
            var deliveryObject = convertor.Deserialize(realBody, deliveryObjectType, useCompressed, useEncrypted);
            return deliveryObject;
        }

        private IIncomingEnvelope CreateIncomeEnvelopInstance(Type messageTypeType, Type deliveryObjectType, object deliveryObject, string from, MessageToken messageToken, DateTimeOffset created)
        {
            var type = typeof(IncomingEnvelope<,>).MakeGenericType(messageTypeType, deliveryObjectType);
            var instance = (IIncomingEnvelope)Activator.CreateInstance(type, messageTypeType, deliveryObject, from, messageToken, created);
            return instance;
        }

        private void RedirectUnprocessedMessage(IDeliveryMessage baseMessage, IDeliveryContext deliveryContext, string reason, string detail)
        {
            while(true)
            {
                try
                {
                    var redirectMessage = this._redirectChannel.CreateMessage();

                    redirectMessage.Id = baseMessage.Id;
                    redirectMessage.AppId = baseMessage.AppId;
                    redirectMessage.Body = baseMessage.Body;
                    redirectMessage.ContentEncoding = baseMessage.ContentEncoding;
                    redirectMessage.ContentType = baseMessage.ContentType;
                    redirectMessage.CorrelationId = baseMessage.CorrelationId;
                    redirectMessage.Headers = baseMessage.Headers;
                    redirectMessage.Type = baseMessage.Type;
                    redirectMessage.Headers["RedirectReason"] = reason;
                    redirectMessage.Headers["RedirectDetail"] = detail;

                    var localExchangeName = $"E.DataBus.[{_config.Name}].Local.[{_config.Address}].[v{_config.ApiVersion}]";

                    this._redirectChannel.Publish(localExchangeName, "RK.[Unprocessed]", redirectMessage);

                    return;
                }
                catch (Exception e)
                {
                    this._logger.Exception(BusContexts.MessageSending, $"An error occurred while redirection message to local Unprocessed queue: Tag ='{this._tag}'", e, this);

                    Thread.Sleep(this._redirectionTimeout);
                }
            }
        }

        private void RedirectRejectedMessage(IDeliveryMessage baseMessage, IDeliveryContext deliveryContext, string reason, string detail)
        {
            while (true)
            {
                try
                {
                    var redirectMessage = this._redirectChannel.CreateMessage();

                    redirectMessage.Id = baseMessage.Id;
                    redirectMessage.AppId = baseMessage.AppId;
                    redirectMessage.Body = baseMessage.Body;
                    redirectMessage.ContentEncoding = baseMessage.ContentEncoding;
                    redirectMessage.ContentType = baseMessage.ContentType;
                    redirectMessage.CorrelationId = baseMessage.CorrelationId;
                    redirectMessage.Headers = baseMessage.Headers;
                    redirectMessage.Type = baseMessage.Type;
                    redirectMessage.Headers["RedirectReason"] = reason;
                    redirectMessage.Headers["RedirectDetail"] = detail;

                    var localExchangeName = $"E.DataBus.[{_config.Name}].Local.[{_config.Address}].[v{_config.ApiVersion}]";

                    this._redirectChannel.Publish(localExchangeName, "RK.[Rejected]", redirectMessage);

                    return;
                }
                catch (Exception e)
                {
                    this._logger.Exception(BusContexts.MessageSending, $"An error occurred while redirection message to local Rejected queue: Tag ='{this._tag}'", e, this);

                    Thread.Sleep(this._redirectionTimeout);
                }
            }
        }

        public void Join(Connection connection)
        {
            this._channel = connection.CreateChannel();
            this._redirectChannel = connection.CreateChannel();
            this._channel.JoinConsumer(this._queueName, this._tag, this);
        }

        public void UnJoin()
        {
            if (this._channel != null)
            {
                try
                {
                    _channel.UnjoinConsumer(this._tag);
                    _channel.Dispose();
                }
                catch (Exception e)
                {
                    this._logger.Exception(BusContexts.Disposing, $"An error occurred while disposing object", e, this);
                }
                _channel = null;
            }

            if (this._redirectChannel != null)
            {
                try
                {
                    _redirectChannel.Dispose();
                }
                catch (Exception e)
                {
                    this._logger.Exception(BusContexts.Disposing, $"An error occurred while disposing object", e, this);
                }
                _redirectChannel = null;
            }
        }
    }
}
