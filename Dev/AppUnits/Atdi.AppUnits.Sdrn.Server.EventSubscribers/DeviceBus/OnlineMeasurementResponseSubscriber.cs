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
using Atdi.Contracts.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server.Events.OnlineMeasurement;

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    [SubscriptionEvent(EventName = "OnOnlineMeasurementResponseDeviceBusEvent", SubscriberName = "OnlineMeasurementResponseSubscriber")]
    public class OnlineMeasurementResponseSubscriber : SubscriberBase<DM.OnlineMeasurementResponseData>
    {

        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IStatistics _statistics;
        private readonly IEventEmitter _eventEmitter;

        private readonly IStatisticCounter _messageProcessingHitsCounter;
        private readonly IStatisticCounter _hitsCounter;
        private readonly IStatisticCounter _errorsCounter;

        public OnlineMeasurementResponseSubscriber(
            IEventEmitter eventEmitter,
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
            this._eventEmitter = eventEmitter;
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
                    bool linkOnlineMesurementExists = false;
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        if (response.Conformed)
                        {
                            var update = this._dataLayer.GetBuilder<MD.IOnlineMesurement>()
                                .Update()
                                .Where(c => c.Id, ConditionOperator.Equal, response.OnlineMeasId)
                                .Where(c => c.StatusCode, ConditionOperator.Equal, (byte)1) // WaiteSensor 
                                .SetValue(c => c.StatusCode, (byte)4) // SonsorReady 
                                .SetValue(c => c.StatusNote, $"SonsorReady: The sensor is waiting the client request")
                                .SetValue(c => c.StartTime, DateTimeOffset.Now)
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
                                .SetValue(c => c.StatusNote, $"DeniedBySensor: The device did not confirm the online measurement: {response.Message}") //DeniedBySensor
                                .SetValue(c => c.SensorToken, null)
                                .SetValue(c => c.WebSocketUrl, null);

                            scope.Executor.Execute(update);
                        }


                        var linkOnlineMesurementQuery = _dataLayer.GetBuilder<MD.ILinkOnlineMesurement>()
                        .From()
                        .Select(c => c.ONLINE_MEAS.Id)
                        .Select(c => c.Id)
                        .Where(c => c.ONLINE_MEAS.Id, ConditionOperator.Equal, response.OnlineMeasId);
                        linkOnlineMesurementExists = scope.Executor.ExecuteAndFetch(linkOnlineMesurementQuery, reader =>
                            {
                                var exists = reader.Read();
                                if (exists)
                                {
                                    exists = reader.GetValue(c => c.ONLINE_MEAS.Id) == response.OnlineMeasId;
                                }
                                return exists;
                            });
                    }
                    

                    if (linkOnlineMesurementExists == true)
                    {
                        var initEvent = new OnOnlineMeasurementResponseDevice(this.GetType().FullName)
                        {
                            OnlineMeasId = response.OnlineMeasId,
                            Conformed = response.Conformed,
                            Message = response.Message,
                            Token = response.Token,
                            WebSocketUrl = response.WebSocketUrl
                        };
                        this._eventEmitter.Emit(initEvent);
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
