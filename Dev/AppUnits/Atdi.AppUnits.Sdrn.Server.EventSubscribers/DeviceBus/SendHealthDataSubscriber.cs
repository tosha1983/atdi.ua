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
using Atdi.DataModels.Sdrns.Server.Events;

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    [SubscriptionEvent(EventName = "OnSendHealthDataDeviceBusEvent", SubscriberName = "SendHealthDataSubscriber")]
    public class SendHealthDataSubscriber : SubscriberBase<DM.SendHealthData>
    {

        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ISdrnServerEnvironment _environment;
        private readonly IEventEmitter _eventEmitter;
        private readonly ISdrnMessagePublisher _messagePublisher;
        private readonly IStatistics _statistics;

        private readonly IStatisticCounter _mpHitsCounter;
        private readonly IStatisticCounter _hitsCounter;
        private readonly IStatisticCounter _errorsCounter;

        public SendHealthDataSubscriber(
	        IEventEmitter eventEmitter,
			ISdrnMessagePublisher messagePublisher, 
            IMessagesSite messagesSite, 
            IDataLayer<EntityDataOrm> dataLayer, 
            ISdrnServerEnvironment environment,
            IStatistics statistics,
            ILogger logger) 
            : base(messagesSite, logger)
        {
	        this._eventEmitter = eventEmitter;
	        this._messagePublisher = messagePublisher;
            this._dataLayer = dataLayer;
            this._environment = environment;
            this._statistics = statistics;
            if (this._statistics != null)
            {
                this._mpHitsCounter = _statistics.Counter(Monitoring.Counters.MessageProcessingHits);
                this._hitsCounter = _statistics.Counter(Monitoring.Counters.SendHealthDataHits);
                this._errorsCounter = _statistics.Counter(Monitoring.Counters.SendHealthDataErrors);
            }
        }

        protected override void Handle(string sensorName, string sensorTechId, DM.SendHealthData deliveryObject, long messageId)
        {
	        var receivedTime = DateTimeOffset.Now;

            this._mpHitsCounter?.Increment();
            this._hitsCounter?.Increment();
            try
            {
                using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                {
	                var insQuery = this._dataLayer.GetBuilder<MD.IHealthLogData>().Insert();
                    insQuery.SetValue(c => c.DispatchTime, deliveryObject.CreatedTime);
                    insQuery.SetValue(c => c.ReceivedTime, receivedTime);
                    insQuery.SetValue(c => c.SourceTypeCode, (byte)deliveryObject.ElementType);
                    insQuery.SetValue(c => c.SourceTypeName, deliveryObject.ElementType.ToString());
                    insQuery.SetValue(c => c.SourceInstance, sensorName);
                    insQuery.SetValue(c => c.SourceTechId, sensorTechId);
                    insQuery.SetValue(c => c.SourceHost, deliveryObject.HostName);
					insQuery.SetValue(c => c.EventCode, (byte)deliveryObject.Event);
                    insQuery.SetValue(c => c.EventName, deliveryObject.Event.ToString());
                    insQuery.SetValue(c => c.EventNote, deliveryObject.Note);
					insQuery.SetValue(c => c.JsonData, deliveryObject.JsonData);

					var health = scope.Executor.Execute<MD.IHealthLogData_PK>(insQuery);

					var detailQuery = this._dataLayer.GetBuilder<MD.IHealthLogDetail>()
							.Insert()
							.SetValue(c => c.HEALTH.Id, health.Id)
							.SetValue(c => c.CreatedDate, receivedTime)
							.SetValue(c => c.Message, "Received health data from Device Bus")
							.SetValue(c => c.Note, $"SensorName='{sensorName}'; TechId='{sensorTechId}'; MessageId=#{messageId}")
							.SetValue(c => c.SiteTypeCode, (byte) MD.EnvironmentElementTypeCode.SdrnServer)
							.SetValue(c => c.SiteTypeName, MD.EnvironmentElementTypeCode.SdrnServer.ToString())
							.SetValue(c => c.SiteInstance, this._environment.ServerInstance)
							.SetValue(c => c.ThreadId , System.Threading.Thread.CurrentThread.ManagedThreadId)
							.SetValue(c => c.Source , "SendHealthDataSubscriber")
							.SetValue(c => c.SiteHost, Environment.MachineName)
						;
					scope.Executor.Execute(detailQuery);

					// нужно пульнуть эвент что пришли данно об элементе инфраструктуры
					var busEvent = new ReceivedHealthDataEvent()
					{
						HealthId = health.Id,
						Source = "SendHealthDataSubscriber"
					};
					_eventEmitter.Emit(busEvent);
				}
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, Categories.MessageProcessing, e, this);
				this._errorsCounter?.Increment();
            }
        }
    }
}
