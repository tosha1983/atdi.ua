using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes
{

    [SubscriptionEvent(EventName = "OnEvent1", SubscriberName = "Subscriber1")]
    [SubscriptionEvent(EventName = "OnEvent2", SubscriberName = "Subscriber2")]
    [SubscriptionEvent(EventName = "OnEvent3", SubscriberName = "Subscriber3")]
    [SubscriptionEvent(EventName = "OnEvent4", SubscriberName = "Subscriber4")]
    [SubscriptionEvent(EventName = "OnEvent5", SubscriberName = "Subscriber5")]
    [SubscriptionEvent(EventName = "OnEvent6", SubscriberName = "Subscriber6")]
    
    public class OnRegisteredNewSensor : IEventSubscriber<Event>
    {
        private readonly IEventEmitter eventEmitter;
        private readonly ILogger _logger;

        public OnRegisteredNewSensor(IEventEmitter eventEmitter, ILogger logger)
        {
            this.eventEmitter = eventEmitter;
            this._logger = logger;
        }

        public void Notify(Event @event)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.Notify, this))
            {
               

                if (@event.Name == "OnEvent1")
                {
                    this.eventEmitter.Emit("OnEvent2", "OnRegisteredNewSensorProcess");
                    this.eventEmitter.Emit("OnEvent3", "OnRegisteredNewSensorProcess");
                    this.eventEmitter.Emit("OnEvent4", "OnRegisteredNewSensorProcess");
                    this.eventEmitter.Emit("OnEvent5", "OnRegisteredNewSensorProcess");
                    this.eventEmitter.Emit("OnEvent6", "OnRegisteredNewSensorProcess");
                    this.eventEmitter.Emit("OnEvent7", "OnRegisteredNewSensorProcess");
                }
                else if(@event.Name == "OnEvent2")
                {
                    this.eventEmitter.Emit("OnEvent3", "OnRegisteredNewSensorProcess");
                    this.eventEmitter.Emit("OnEvent4", "OnRegisteredNewSensorProcess");
                    this.eventEmitter.Emit("OnEvent5", "OnRegisteredNewSensorProcess");
                    this.eventEmitter.Emit("OnEvent6", "OnRegisteredNewSensorProcess");
                    this.eventEmitter.Emit("OnEvent7", "OnRegisteredNewSensorProcess");
                }
                else if (@event.Name == "OnEvent3")
                {
                    this.eventEmitter.Emit("OnEvent4", "OnRegisteredNewSensorProcess");
                    this.eventEmitter.Emit("OnEvent5", "OnRegisteredNewSensorProcess");
                    this.eventEmitter.Emit("OnEvent6", "OnRegisteredNewSensorProcess");
                    this.eventEmitter.Emit("OnEvent7", "OnRegisteredNewSensorProcess");
                }
                else if (@event.Name == "OnEvent4")
                {
                    this.eventEmitter.Emit("OnEvent5", "OnRegisteredNewSensorProcess");
                    this.eventEmitter.Emit("OnEvent6", "OnRegisteredNewSensorProcess");
                    this.eventEmitter.Emit("OnEvent7", "OnRegisteredNewSensorProcess");
                }
                else if (@event.Name == "OnEvent5")
                {
                    this.eventEmitter.Emit("OnEvent6", "OnRegisteredNewSensorProcess");
                    this.eventEmitter.Emit("OnEvent7", "OnRegisteredNewSensorProcess");
                }
                else if (@event.Name == "OnEvent6")
                {
                    this.eventEmitter.Emit("OnEvent7", "OnRegisteredNewSensorProcess");
                }
            }
        }
    }
}
