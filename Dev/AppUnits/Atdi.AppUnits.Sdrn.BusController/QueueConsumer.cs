using Atdi.Platform.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.DependencyInjection;
using Atdi.DataModels.Sdrns.Device;
using Newtonsoft.Json;

namespace Atdi.AppUnits.Sdrn.BusController
{
    public class QueueConsumer : DefaultBasicConsumer
    {
        private readonly EventCategory _eventCategory;
        private readonly EventContext _eventContext;

        private readonly string _queueName;
        private readonly string _consumerName;
        private readonly SdrnServerDescriptor _serverDescriptor;
        private readonly IServicesResolver _servicesResolver;
        private readonly ILogger _logger;

        public QueueConsumer(string consumerName, string queueName, SdrnServerDescriptor serverDescriptor, IModel channel, IServicesResolver servicesResolver, ILogger logger) : base(channel)
        {
            this._consumerName = consumerName;
            this._queueName = queueName;
            this._serverDescriptor = serverDescriptor;
            this._servicesResolver = servicesResolver;
            this._logger = logger;
            this._eventCategory = (EventCategory)$"Rabbit MQ: {this._serverDescriptor.RabbitMqHost}";
            this._eventContext = $"SDRN.Server.[{_serverDescriptor.ServerInstance}].Consumer.{consumerName}";
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            this._logger.Verbouse(this._eventContext, this._eventCategory, $"The message {deliveryTag} is handled successfully: Type = '{properties.Type}'");

            var sensorDescriptor = this.GetSensorDescriptor(properties);
            if (this._serverDescriptor.ServerInstance != sensorDescriptor.SdrnServer)
            {
                this.Model.BasicAck(deliveryTag, false);
                return;
            }

            if (properties.Type == "RegisterSensor")
            {
                var sensor = DeserializeObjectFromByteArray<Sensor>(body);
                this._logger.Verbouse(this._eventContext, this._eventCategory, $"The object is '{sensor}'");
                this.Model.BasicAck(deliveryTag, false);

                var result = new SensorRegistrationResult
                {
                    Status = "Ok",
                    SdrnServer = sensorDescriptor.SdrnServer,
                    SensorName = sensorDescriptor.SensorName,
                    EquipmentTechId = sensorDescriptor.EquipmentTechId
                };

                var publisherConnector = this._servicesResolver.Resolve<PublisherBusConnector>();
                publisherConnector.Publish(sensorDescriptor, "SendRegistrationResult", result, properties.CorrelationId);
                return;
            }

            if (properties.Type == "UpdateSensor")
            {
                var sensor = DeserializeObjectFromByteArray<Sensor>(body);
                this._logger.Verbouse(this._eventContext, this._eventCategory, $"The object is '{sensor}'");
                this.Model.BasicAck(deliveryTag, false);

                var result = new SensorUpdatingResult
                {
                    Status = "Ok",
                    SdrnServer = sensorDescriptor.SdrnServer,
                    SensorName = sensorDescriptor.SensorName,
                    EquipmentTechId = sensorDescriptor.EquipmentTechId
                };

                var publisherConnector = this._servicesResolver.Resolve<PublisherBusConnector>();
                publisherConnector.Publish(sensorDescriptor, "SendSensorUpdatingResult", result, properties.CorrelationId);


                // send command
                var commnad = new DeviceCommand
                {
                    CommandId = Guid.NewGuid().ToString(),
                    Command = "CheckSensorActivity",
                    SdrnServer = sensorDescriptor.SdrnServer,
                    SensorName = sensorDescriptor.SensorName,
                    EquipmentTechId = sensorDescriptor.EquipmentTechId
                };

                publisherConnector.Publish(sensorDescriptor, "SendCommand", commnad, commnad.CommandId);

                return;

            }

            if (properties.Type == "SendCommandResult")
            {
                this.Model.BasicAck(deliveryTag, false);

                var publisherConnector = this._servicesResolver.Resolve<PublisherBusConnector>();
                var task = new MeasTask
                {
                    TaskId =  Guid.NewGuid().ToString(),
                    SdrnServer = sensorDescriptor.SdrnServer,
                    SensorName = sensorDescriptor.SensorName,
                    EquipmentTechId = sensorDescriptor.EquipmentTechId
                };

                publisherConnector.Publish(sensorDescriptor, "SendMeasTask", task, task.TaskId);

                return;
            }
            if (properties.Type == "SendMeasResults")
            {
                this.Model.BasicAck(deliveryTag, false);

                var publisherConnector = this._servicesResolver.Resolve<PublisherBusConnector>();
                var entity = new Entity
                {
                    EntityId = Guid.NewGuid().ToString(),
                    Name = "Some entity",
                    EOF = true,
                    PartIndex = 0
                };

                publisherConnector.Publish(sensorDescriptor, "SendEntity", entity, entity.EntityId);

                return;
            }
            if (properties.Type == "SendEntity")
            {
                this.Model.BasicAck(deliveryTag, false);
                return;
            }
            if (properties.Type == "SendEntityPart")
            {
                this.Model.BasicAck(deliveryTag, false);
                return;
            }
            
            
        }

        private SensorDescriptor GetSensorDescriptor(IBasicProperties properties)
        {
            var result = new SensorDescriptor
            {
                SdrnServer = this.GetHeaderValue("SdrnServer", properties),
                SensorName = this.GetHeaderValue("SensorName", properties),
                EquipmentTechId = this.GetHeaderValue("SensorTechId", properties),
            };

            return result;
        }
        private string GetHeaderValue(string key, IBasicProperties properties )
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

        private TObject DeserializeObjectFromByteArray<TObject>(byte[] source)
        {
            if (source == null)
            {
                return default(TObject);
            }

            TObject result = default(TObject);

            var json = Encoding.UTF8.GetString(source);
            result = JsonConvert.DeserializeObject<TObject>(json);

            return result;
        }

        public void Activate()
        {
            try
            {
                this.Model.BasicConsume(_queueName, false, _consumerName, this);

                this._logger.Verbouse(this._eventContext, this._eventCategory, $"The consumer is activated successfully: Queue name = '{this._queueName}'");
            }
            catch(Exception e)
            {
                this._logger.Exception(this._eventContext, this._eventCategory, e);
                throw new InvalidOperationException($"The consumer '{this._consumerName}' is not activated: Queue name = '{_queueName}'", e);
            }
        }
    }
}
