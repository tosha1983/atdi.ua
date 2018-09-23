﻿
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Atdi.Modules.Sdrn.MessageBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal class QueueConsumer : DefaultBasicConsumer
    {
        private readonly string _queueName;
        private readonly EnvironmentDescriptor _environmentDescriptor;
        private readonly MessageDispatcher _messageDispatcher;
        private readonly RabbitMQBus _rabbitMQBus;
        private readonly string _consumerName;
        private readonly BusLogger _logger;

        public QueueConsumer(string consumerName, string queueName, EnvironmentDescriptor environmentDescriptor, MessageDispatcher messageDispatcher, RabbitMQBus rabbitMQBus, IModel channel, BusLogger logger) : base(channel)
        {
            this._consumerName = consumerName;
            this._queueName = queueName;
            this._environmentDescriptor = environmentDescriptor;
            this._messageDispatcher = messageDispatcher;
            this._rabbitMQBus = rabbitMQBus;
            this._logger = logger;
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            this._logger.Verbouse("OnMessage", $"Recieved message #{deliveryTag}: Type = '{properties.Type}'", this);

            var result = _messageDispatcher.OnMessage(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body);

            if (result.Result == Contracts.Api.Sdrn.MessageBus.MessageHandlingResult.Confirmed)
            {
                this.Model.BasicAck(deliveryTag, false);
            }
            else if (result.Result == Contracts.Api.Sdrn.MessageBus.MessageHandlingResult.Reject)
            {
                this.Model.BasicNack(deliveryTag, false, false);
            }
            else if (result.Result == Contracts.Api.Sdrn.MessageBus.MessageHandlingResult.Ignore)
            {
                return;
            }
            else if (result.Result == Contracts.Api.Sdrn.MessageBus.MessageHandlingResult.Received)
            {
                return;
            }
            else if (result.Result == Contracts.Api.Sdrn.MessageBus.MessageHandlingResult.Error)
            {
                var message = new Message
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "Error." + properties.Type,
                    CorrelationId = properties.MessageId,
                    ContentType = properties.ContentType,
                    ContentEncoding = properties.ContentEncoding,
                    Body = body
                };
                message.Headers = new Dictionary<string, object>
                {
                    ["SdrnServer"] = this._environmentDescriptor.SdrnServerInstance,
                    ["SensorName"] = this._environmentDescriptor.SdrnDeviceSensorName,
                    ["SensorTechId"] = this._environmentDescriptor.SdrnDeviceSensorTechId,
                    ["Created"] = DateTime.Now.ToString("o"),
                    ["Source.SdrnServer"] = properties.Headers["SdrnServer"],
                    ["Source.SensorName"] = properties.Headers["SensorName"],
                    ["Source.SensorTechId"] = properties.Headers["SensorTechId"],
                    ["Source.Created"] = properties.Headers["Created"],
                    ["Source.AppId"] = properties.AppId,
                    ["Source.CorrelationId"] = properties.CorrelationId,
                    ["Error.ReasonFailure"] = result.ReasonFailure
                };

                _rabbitMQBus.Publish(_environmentDescriptor.BuildDeviceExchangeName(), this._environmentDescriptor.GetRoutingKeyByContext("errors"), message);
                this.Model.BasicAck(deliveryTag, false);
            }
            else if (result.Result == Contracts.Api.Sdrn.MessageBus.MessageHandlingResult.Trash)
            {
                var message = new Message
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "Trash." + properties.Type,
                    CorrelationId = properties.MessageId,
                    ContentType = properties.ContentType,
                    ContentEncoding = properties.ContentEncoding,
                    Body = body
                };
                message.Headers = new Dictionary<string, object>
                {
                    ["SdrnServer"] = this._environmentDescriptor.SdrnServerInstance,
                    ["SensorName"] = this._environmentDescriptor.SdrnDeviceSensorName,
                    ["SensorTechId"] = this._environmentDescriptor.SdrnDeviceSensorTechId,
                    ["Created"] = DateTime.Now.ToString("o"),
                    ["Source.SdrnServer"] = properties.Headers["SdrnServer"],
                    ["Source.SensorName"] = properties.Headers["SensorName"],
                    ["Source.SensorTechId"] = properties.Headers["SensorTechId"],
                    ["Source.Created"] = properties.Headers["Created"],
                    ["Source.AppId"] = properties.AppId,
                    ["Source.CorrelationId"] = properties.CorrelationId,
                    ["Trash.ReasonFailure"] = result.ReasonFailure
                };

                _rabbitMQBus.Publish(_environmentDescriptor.BuildDeviceExchangeName(), this._environmentDescriptor.GetRoutingKeyByContext("trash"), message);
                this.Model.BasicAck(deliveryTag, false);
            }
        }

        public void Activate()
        {
            try
            {
                this.Model.BasicConsume(_queueName, false, _consumerName, this);

               this._logger.Verbouse("Activate", $"The consumer '{this._consumerName}' is activated successfully: Queue name = '{this._queueName}'", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BusEvents.ExceptionEvent, "Activate", e, this);
                throw new InvalidOperationException($"The consumer '{this._consumerName}' is not activated: Queue name = '{_queueName}'", e);
            }
        }

        public void Deactivate()
        {
            try
            {
                this.Model.BasicCancel(_consumerName);

                this._logger.Verbouse("Deactivate", $"The consumer '{this._consumerName}' is deactivated successfully: Queue name = '{this._queueName}'", this);
            }
            catch (Exception e)
            {
                this._logger.Exception(BusEvents.ExceptionEvent, "Deactivate", e, this);
                throw new InvalidOperationException($"The consumer '{this._consumerName}' is not deactivated: Queue name = '{_queueName}'", e);
            }
        }
    }
}
