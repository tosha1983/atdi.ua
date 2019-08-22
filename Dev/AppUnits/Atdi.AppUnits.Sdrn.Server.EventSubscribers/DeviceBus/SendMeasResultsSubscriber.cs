using Atdi.DataModels.Api.EventSystem;
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
using Atdi.DataModels.Sdrns;
using Atdi.DataModels.Sdrns.Device;
using DM = Atdi.DataModels.Sdrns.Device;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.DataConstraint;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using Atdi.Contracts.Api.EventSystem;
using Atdi.DataModels.Sdrns.Server.Events;
using Atdi.Common;
using Atdi.Platform;
using Atdi.Platform.Caching;

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    [SubscriptionEvent(EventName = "OnSendMeasResultsDeviceBusEvent", SubscriberName = "SendMeasResultsSubscriber")]
    public class SendMeasResultsSubscriber : SubscriberBase<DM.MeasResults>
    {
        class HandleContext
        {
            public long messageId;
            public long resMeasId = 0;
            public string sensorName;
            public string sensorTechId;
            public IDataLayerScope scope;
            public DM.MeasResults measResult;
        }
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IStatistics _statistics;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IEventEmitter _eventEmitter;
        private readonly IQueryExecutor _queryExecutor;

        private readonly IStatisticCounter _messageProcessingHitsCounter;
        private readonly IStatisticCounter _sendMeasResultsErrorsCounter;
        private readonly IStatisticCounter _sendMeasResultsHitsCounter;

        public SendMeasResultsSubscriber(
            IEventEmitter eventEmitter, 
            ISdrnMessagePublisher messagePublisher, 
            IMessagesSite messagesSite, 
            IDataLayer<EntityDataOrm> dataLayer, 
            ISdrnServerEnvironment environment, 
            IStatistics statistics,
            IDataCacheSite cacheSite,
            ILogger logger) 
            : base(messagesSite, logger)
        {
            this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._statistics = statistics;
            this._eventEmitter = eventEmitter;
            this._queryExecutor = this._dataLayer.Executor<SdrnServerDataContext>();

            if (this._statistics != null)
            {
                this._messageProcessingHitsCounter = _statistics.Counter(Monitoring.Counters.MessageProcessingHits);
                this._sendMeasResultsErrorsCounter = _statistics.Counter(Monitoring.Counters.SendMeasResultsErrors);
                this._sendMeasResultsHitsCounter = _statistics.Counter(Monitoring.Counters.SendMeasResultsHits);
            }
        }

        protected override void Handle(string sensorName, string sensorTechId, DM.MeasResults deliveryObject, long messageId)
        {
            using (this._logger.StartTrace(Contexts.ThisComponent, Categories.MessageProcessing, this))
            {
                this._messageProcessingHitsCounter?.Increment();
                this._sendMeasResultsHitsCounter?.Increment();

                var reasonFailure = "";
                try
                {
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        var context = new HandleContext()
                        {
                            messageId = messageId,
                            resMeasId = 0,
                            sensorName = sensorName,
                            sensorTechId = sensorTechId,
                            scope = scope,
                            measResult = deliveryObject
                        };

                        if (deliveryObject.Measurement == MeasurementType.SpectrumOccupation)
                        {
                            var busEvent = new DevicesBusEvent($"OnSOMeasResultsDeviceBusEvent", "SendMeasResultsSubscriber")
                            {
                                BusMessageId = messageId
                            };
                            _eventEmitter.Emit(busEvent);
                        }
                        else if (deliveryObject.Measurement == MeasurementType.MonitoringStations)
                        {
                            var busEvent = new DevicesBusEvent($"OnMSMeasResultsDeviceBusEvent", "SendMeasResultsSubscriber")
                            {
                                BusMessageId = messageId
                            };
                            _eventEmitter.Emit(busEvent);
                        }
                        else if (deliveryObject.Measurement == MeasurementType.Signaling)
                        {
                            var busEvent = new DevicesBusEvent($"OnSGMeasResultsDeviceBusEvent", "SendMeasResultsSubscriber")
                            {
                                BusMessageId = messageId
                            };
                            _eventEmitter.Emit(busEvent);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unsupported MeasurementType '{deliveryObject.Measurement}'");
                        }
                    }
                }
                catch (Exception e)
                {
                    this._sendMeasResultsErrorsCounter?.Increment();
                    this._logger.Exception(Contexts.ThisComponent, Categories.MessageProcessing, e, this);
                    //status = SdrnMessageHandlingStatus.Error;
                    reasonFailure = e.StackTrace;
                }
            }
        }
    }
}
