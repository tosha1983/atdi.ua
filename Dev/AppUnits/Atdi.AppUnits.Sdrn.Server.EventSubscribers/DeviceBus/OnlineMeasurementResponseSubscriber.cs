using Atdi.DataModels.Api.EventSystem;
using DM = Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.DataConstraint;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using Atdi.Platform;

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    [SubscriptionEvent(EventName = "OnOnlineMeasurementResponseDeviceBusEvent", SubscriberName = "OnlineMeasurementResponseSubscriber")]
    public class OnlineMeasurementResponseSubscriber : SubscriberBase<DM.OnlineMeasurementResponseData>
    {

        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IStatistics _statistics;

        private readonly IStatisticCounter _messageProcessingHitsCounter;
        private readonly IStatisticCounter _hitsCounter;
        private readonly IStatisticCounter _errorsCounter;

        public OnlineMeasurementResponseSubscriber(
            ISdrnMessagePublisher messagePublisher, 
            IMessagesSite messagesSite, 
            IDataLayer<EntityDataOrm> dataLayer, 
            ISdrnServerEnvironment environment,
            IStatistics statistics,
            ILogger logger) 
            : base(messagesSite, logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._statistics = statistics;
            if (this._statistics != null)
            {
                this._messageProcessingHitsCounter = _statistics.Counter(Monitoring.Counters.MessageProcessingHits);
                this._hitsCounter = _statistics.Counter(Monitoring.Counters.OnlineMeasurementResponseHits);
                this._errorsCounter = _statistics.Counter(Monitoring.Counters.OnlineMeasurementResponseErrors);
            }
        }

        protected override void Handle(string sensorName, string sensorTechId, DM.OnlineMeasurementResponseData deliveryObject, long messageId)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
            {
                this._messageProcessingHitsCounter?.Increment();
                this._hitsCounter?.Increment();

                try
                {
                    var response = deliveryObject;

                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        if (response.Conformed)
                        {
                            var update = this._dataLayer.GetBuilder<MD.IOnlineMesurement>()
                                .Update()
                                .Where(c => c.Id, ConditionOperator.Equal, response.OnlineMeasId)
                                .Where(c => c.StatusCode, ConditionOperator.Equal, (byte)1) // WaiteSensor 
                                .SetValue(c => c.StatusCode, (byte)4) // SonsorReady 
                                .SetValue(c => c.SensorToken, response.Token)
                                .SetValue(c => c.WebSocketUrl, response.WebSocketUrl);

                            scope.Executor.Execute(update);
                        }
                        else
                        {
                            var update = this._dataLayer.GetBuilder<MD.IOnlineMesurement>()
                                .Update()
                                .Where(c => c.Id, ConditionOperator.Equal, response.OnlineMeasId)
                                .Where(c => c.StatusCode, ConditionOperator.Equal, (byte)1)  // WaiteSensor
                                .SetValue(c => c.StatusCode, (byte)3) //DeniedBySensor
                                .SetValue(c => c.SensorToken, response.Token)
                                .SetValue(c => c.WebSocketUrl, response.WebSocketUrl);

                            scope.Executor.Execute(update);
                        }
                    }
                }
                catch (Exception e)
                {
                    this._errorsCounter?.Increment();
                    this._logger.Exception(Contexts.ThisComponent, Categories.MessageProcessing, e, this);
                    throw new InvalidOperationException($"An unexpected error occurred while processing messages '{e.Message}': Message ID = #{messageId}, Sensor = '{sensorName}', TechID = '{sensorTechId}'");
                }
            }
        }
    }
}
