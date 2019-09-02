using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Api.EventSystem;
using Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Handlers;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Sdrns.Server;


namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Subscribes
{
    [SubscriptionEvent(EventName = "OnRegisterAggregationServerEvent", SubscriberName = "SubscriberOnRegisterAggregationServerEvent")]
    public class OnRegisterAggregationServerEvent : IEventSubscriber<OnRegisterAggregationServer>
    {
        private readonly ILogger _logger;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IPublisher _publisher;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;

        public OnRegisterAggregationServerEvent(IPublisher publisher, IDataLayer<EntityDataOrm> dataLayer, ILogger logger, ISdrnServerEnvironment environment)
        {
            this._logger = logger;
            this._environment = environment;
            this._publisher = publisher;
            this._dataLayer = dataLayer;
        }


       

        public void Notify(OnRegisterAggregationServer @event)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.OnRegisterAggregationServerEvent, this))
            {
                var loadSensor = new LoadSensor(this._dataLayer, this._logger);
                var saveSensor = new SaveSensor(this._dataLayer, this._logger);
                saveSensor.UpdateFieldAggregationServerInstanceInSensor(@event.SensorName, @event.EquipmentTechId, this._environment.ServerInstance);
                var sensor = loadSensor.LoadObjectSensor(@event.SensorName, @event.EquipmentTechId);
                var retEnvelope = this._publisher.CreateEnvelope<SendSensorFromAggregationToMasterServer, Sensor>();
                retEnvelope.To = this._environment.MasterServerInstance;
                retEnvelope.DeliveryObject = sensor;
                retEnvelope.DeliveryObject.AggregationServerInstance = this._environment.ServerInstance;
                this._publisher.Send(retEnvelope);
            }
        }
    }
}
