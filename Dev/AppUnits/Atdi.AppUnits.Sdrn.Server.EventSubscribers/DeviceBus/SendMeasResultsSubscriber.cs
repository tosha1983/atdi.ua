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
        //class HandleContext
        //{
        //    public long messageId;
        //    public long resMeasId = 0;
        //    public string sensorName;
        //    public string sensorTechId;
        //    public IDataLayerScope scope;
        //    public DM.MeasResults measResult;
        //}
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
                        DevicesBusEvent busEvent;

                        if (deliveryObject.Measurement == MeasurementType.SpectrumOccupation)
                        {
                            busEvent = new DevicesBusEvent($"OnSOMeasResultsDeviceBusEvent", "SendMeasResultsSubscriber") { BusMessageId = messageId };
                        }
                        else if (deliveryObject.Measurement == MeasurementType.MonitoringStations)
                        {
                            busEvent = new DevicesBusEvent($"OnMSMeasResultsDeviceBusEvent", "SendMeasResultsSubscriber") { BusMessageId = messageId };
                        }
                        else if (deliveryObject.Measurement == MeasurementType.Signaling)
                        {
                            busEvent = new DevicesBusEvent($"OnSGMeasResultsDeviceBusEvent", "SendMeasResultsSubscriber") { BusMessageId = messageId };
                        }
                        else if (deliveryObject.Measurement == MeasurementType.Tdoa)
                        {
                            busEvent = new DevicesBusEvent($"OnTdoaMeasResultsDeviceBusEvent", "SendMeasResultsSubscriber") { BusMessageId = messageId };
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unsupported MeasurementType '{deliveryObject.Measurement}'");
                        }

                        var logQueryInsert = this._dataLayer.GetBuilder<MD.IAmqpMessageLog>()
							.Insert()
	                        .SetValue(c => c.MESSAGE.Id, messageId)
	                        .SetValue(c => c.StatusCode, (byte)2)
	                        .SetValue(c => c.StatusName, "Processing")
	                        .SetValue(c => c.StatusNote, $"Decode meas result. Will be sent event '{busEvent.Name}'")
	                        .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
	                        .SetValue(c => c.ThreadId, System.Threading.Thread.CurrentThread.ManagedThreadId)
	                        .SetValue(c => c.Source,
		                        $"SDRN.SendMeasResultsSubscriber:[{deliveryObject?.Measurement}][Handle]");
                        scope.Executor.Execute(logQueryInsert);

						var builderUpdateAmqpMessage = this._dataLayer.GetBuilder<MD.IAmqpMessage>().Update();
                        builderUpdateAmqpMessage.SetValue(c => c.StatusCode, (byte)3);
                        builderUpdateAmqpMessage.Where(c => c.Id, ConditionOperator.Equal, messageId);
                        scope.Executor.Execute(builderUpdateAmqpMessage);

                        _eventEmitter.Emit(busEvent);
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
