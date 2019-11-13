using System;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Platform.Workflows;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.DataModels.Api.EventSystem;
using Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Handlers;
using Atdi.Contracts.Api.DataBus;




namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Subscribes
{
    public class RegisterSensorPipelineHandler : IPipelineHandler<RegisterSensorSendEvent, RegisterSensorSendEvent>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ILogger _logger;
        private readonly IPublisher _publisher;


        public RegisterSensorPipelineHandler(IEventEmitter eventEmitter, IDataLayer<EntityDataOrm> dataLayer, ISdrnServerEnvironment environment, ISdrnMessagePublisher messagePublisher, IPublisher publisher, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
            this._environment = environment;
            this._publisher = publisher;
        }


        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public RegisterSensorSendEvent Handle(RegisterSensorSendEvent data, IPipelineContext<RegisterSensorSendEvent, RegisterSensorSendEvent> context)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.OnRegisterAggregationServerEvent, this))
            {
                var loadSensor = new LoadSensor(this._dataLayer, this._logger);
                var saveSensor = new SaveSensor(this._dataLayer, this._logger);
                saveSensor.UpdateFieldAggregationServerInstanceInSensor(data.SensorName, data.EquipmentTechId, this._environment.ServerInstance);
                var sensor = loadSensor.LoadObjectSensor(data.SensorName, data.EquipmentTechId);
                var retEnvelope = this._publisher.CreateEnvelope<SendSensorFromAggregationToMasterServer, Sensor>();
                retEnvelope.To = this._environment.MasterServerInstance;
                retEnvelope.DeliveryObject = sensor;
                retEnvelope.DeliveryObject.AggregationServerInstance = this._environment.ServerInstance;
                this._publisher.Send(retEnvelope);
                return new RegisterSensorSendEvent()
                {
                     EquipmentTechId = sensor.Equipment.TechId,
                     SensorName = sensor.Name
                };
            }
        }
    }
}
