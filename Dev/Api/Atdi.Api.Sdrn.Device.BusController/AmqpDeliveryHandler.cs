﻿
using Atdi.Modules.AmqpBroker;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Text;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Modules.Sdrn.DeviceBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal sealed class AmqpDeliveryHandler : IDeliveryHandler, IDisposable
    {
        private readonly string _tag;
        private readonly DeviceBusConfig _config;
        private readonly string _queueName;
        private readonly Dictionary<string, List<MessageHandlerDescriptor>> _handlers;
        private readonly BusMessagePacker _messagePacker;

        private readonly BusLogger _logger;
        private Channel _redirectChannel;
        private Channel _channel;

        public AmqpDeliveryHandler(string dispatherTag, DeviceBusConfig config, string queueName, Dictionary<string, List<MessageHandlerDescriptor>> handlers, BusMessagePacker messagePacker, BusLogger logger)
        {
            this._tag = $"[{dispatherTag}].[{this.GetHashCode()}]";
            this._config = config;
            this._queueName = queueName;
            this._handlers = handlers;
            this._messagePacker = messagePacker;
            this._logger = logger;
        }

        public void Dispose()
        {
            this.Detach();
        }

        public HandlingResult Handle(IDeliveryMessage message, IDeliveryContext deliveryContext)
        {
            this._logger.Verbouse("DeviceBus.MessageHandling", $"The message is handling: {message}, {deliveryContext}", this);

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

                if (string.IsNullOrEmpty(message.AppId))
                {
                    errors.AppendLine("Undefined Application ID");
                }

                if (string.IsNullOrEmpty(message.Id))
                {
                    errors.AppendLine("Undefined Message ID");
                }

                var msgSdrnServer = message.GetHeaderValue("SdrnServer");
                var msgSensorName = message.GetHeaderValue("SensorName");
                var msgSensorTechId = message.GetHeaderValue("SensorTechId");

                if (!_config.SdrnServerInstance.Equals(msgSdrnServer, StringComparison.OrdinalIgnoreCase))
                {
                    errors.AppendLine("Incorrect server");
                }

                if (!_config.SdrnDeviceSensorName.Equals(msgSensorName, StringComparison.OrdinalIgnoreCase))
                {
                    errors.AppendLine("Incorrect sensor name");
                }

                if (!_config.SdrnDeviceSensorTechId.Equals(msgSensorTechId, StringComparison.OrdinalIgnoreCase))
                {
                    errors.AppendLine("Incorrect sensor tech ID");
                }

                if (!this._handlers.ContainsKey(message.Type))
                {
                    errors.AppendLine("Unsupported message type");
                }

                if (errors.Length > 0)
                {
                    throw new InvalidOperationException(
                        $"Incorrect message header: {Environment.NewLine + errors.ToString()}");
                }

                //throw new InvalidOperationException("Testing redirection by an error");
                var protocol = message.GetHeaderValue(Protocol.Header.Protocol);
                var bodyAqName = message.GetHeaderValue(Protocol.Header.BodyAqName);

                var messageObject = BusMessagePacker.Unpack(message.ContentType, message.ContentEncoding, protocol,
                    message.Body, bodyAqName, _config.DeviceBusSharedSecretKey);

                var handlers = this._handlers[message.Type];
                var messageToken = new MessageToken(message.Id, message.Type);

                foreach (var handler in handlers)
                {
                    var receivedMessageParam = Activator.CreateInstance(
                        handler.ReceivedMessageGenericType,
                        new object[]
                        {
                            messageToken,
                            messageObject,
                            DateTime.Now,
                            message.CorrelationId
                        });

                    try
                    {
                        handler.OnHandleMethod.Invoke(handler.Instance, new object[] {receivedMessageParam});
                    }
                    catch (Exception e)
                    {
                        _logger.Exception("DeviceBus.MessageDispatching", "", e, this);
                        throw new InvalidOperationException($"Error occurred while processing the message: {e.Message}",
                            e);
                    }

                    var result = receivedMessageParam as IMessageResult;

                    if (result.Result == MessageHandlingResult.Received)
                    {
                        continue;
                    }
                    else if (result.Result == MessageHandlingResult.Confirmed)
                    {
                        break;
                    }
                    else if (result.Result == MessageHandlingResult.Ignore)
                    {
                        continue;
                    }
                    else if (result.Result == MessageHandlingResult.Reject)
                    {
                        return HandlingResult.Reject;
                    }
                    else if (result.Result == MessageHandlingResult.Trash)
                    {
                        this.RedirectMessageToServerQueue("trash", message, deliveryContext, "Trash", result.ReasonFailure, null);
                        return HandlingResult.Confirm;
                    }
                    else if (result.Result == MessageHandlingResult.Error)
                    {
                        this.RedirectMessageToServerQueue("errors", message, deliveryContext, "Error", result.ReasonFailure, null);
                        return HandlingResult.Confirm;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
                return HandlingResult.Confirm;
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.MessageHandling", $"An error occurred while handling message: {message}, {deliveryContext}", e, this);
                this.RedirectMessageToServerQueue("errors", message, deliveryContext, "Exception",  $"{e.Message}", e.ToString());
                return HandlingResult.Confirm;
            }
            finally
            {
                this._logger.Verbouse("DeviceBus.MessageHandling", $"The message was handled: {message}, {deliveryContext}", this);
            }
        }

        private void RedirectMessageToServerQueue(string context, IDeliveryMessage baseMessage, IDeliveryContext deliveryContext, string reason, string message, string detail)
        {
            var exchange = this._config.BuildDeviceExchangeName();
            var routing = this._config.GetRoutingKeyByContext(context);

            try
            {
                var redirectMessage = this._redirectChannel.CreateMessage();

                redirectMessage.Id = baseMessage.Id;
                redirectMessage.AppId = baseMessage.AppId;
                redirectMessage.Body = baseMessage.Body;
                redirectMessage.ContentEncoding = baseMessage.ContentEncoding;
                redirectMessage.ContentType = baseMessage.ContentType;
                redirectMessage.CorrelationId = baseMessage.CorrelationId;
                redirectMessage.Headers = new Dictionary<string, object>(baseMessage.Headers);
                redirectMessage.Type = baseMessage.Type;

                redirectMessage.Headers["Redirect.Reason"] = reason;
                redirectMessage.Headers["Redirect.Message"] = message;
                redirectMessage.Headers["Redirect.Detail"] = detail;

                this._redirectChannel.Publish(exchange, routing, redirectMessage);
            }
            catch (Exception e)
            {
                this._logger.Exception("DeviceBus.Redirection", $"An error occurred while redirection message to server {context} queue: Exchange='{exchange}', Routing='{routing}', Consumer='{this._tag}', Reason='{reason}', ErrMessage='{message}'", e, this);
            }

        }

        public void Attach(Connection connection)
        {
            this._channel = connection.CreateChannel();
            this._redirectChannel = connection.CreateChannel();
            this._channel.AttachConsumer(this._queueName, this._tag, this);
        }

        public void Detach()
        {
            if (this._channel != null)
            {
                try
                {
                    _channel.DetachConsumer(this._tag);
                    _channel.Dispose();
                }
                catch (Exception e)
                {
                    this._logger.Exception("DeviceBus.ConsumerDetachment", $"An error occurred while detaching consumer: Tag='{_tag}'", e, this);
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
                    this._logger.Exception("DeviceBus.ConsumerDetachment", $"An error occurred while detaching consumer: Tag='{_tag}'", e, this);
                }
                _redirectChannel = null;
            }
        }

        
    }
}
