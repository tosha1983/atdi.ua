using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server.Events.OnlineMeasurement;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DM = Atdi.DataModels.Sdrns.Server.Entities;
using System.Threading.Tasks;
using Atdi.DataModels.DataConstraint;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using DEV = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.Api.DataBus;
using Atdi.DataModels.Sdrns.Server;


namespace Atdi.AppUnits.Sdrn.MasterServer.PrimaryHandlers.Subscribes
{
    [SubscriptionEvent(EventName = "OnInitExternalOnlineMeasurement", SubscriberName = "SubscriberOnInitExternalOnlineMeasurementEvent")]
    public class OnInitExternalOnlineMeasurementEvent : IEventSubscriber<OnInitExternalOnlineMeasurement>
    {
        private readonly ILogger _logger;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IPublisher _publisher;

        public OnInitExternalOnlineMeasurementEvent(IDataLayer<EntityDataOrm> dataLayer, ILogger logger, ISdrnServerEnvironment environment, IPublisher publisher)
        {
            this._logger = logger;
            this._environment = environment;
            this._dataLayer = dataLayer;
            this._publisher = publisher;
        }

        public void Notify(OnInitExternalOnlineMeasurement @event)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.OnInitExternalOnlineMeasurementEvent, this))
            {
                var retEnvelope = this._publisher.CreateEnvelope<OnlineMeasToAggregationServer, OnlineMeasurementPipebox>();
                retEnvelope.To = @event.AggregationServerInstance;
                retEnvelope.DeliveryObject = new OnlineMeasurementPipebox()
                {
                    OnlineMeasId = @event.OnlineMeasId,
                    ServerToken = @event.ServerToken,
                    AggregationServerInstance = @event.AggregationServerInstance,
                    SensorName = @event.SensorName,
                    SensorTechId = @event.SensorTechId,
                    Period = @event.Period
                };
                this._publisher.Send(retEnvelope);
            }
        }
    }
}
