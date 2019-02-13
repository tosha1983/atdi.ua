using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes
{
    [SubscriptionEvent(EventName = "OnNewCreateMeasTaskEvent", SubscriberName = "SubscriberCreateMeasTaskProcess")]
    [SubscriptionEvent(EventName = "OnDelCreateMeasTaskEvent", SubscriberName = "SubscriberCreateMeasTaskProcess")]
    [SubscriptionEvent(EventName = "OnStopCreateMeasTaskEvent", SubscriberName = "SubscriberCreateMeasTaskProcess")]
    public class OnCreateMeasTaskProcess : IEventSubscriber<Event>
    {
        private readonly ILogger _logger;

        public OnCreateMeasTaskProcess(ILogger logger)
        {
            this._logger = logger;
        }

        public void Notify(Event @event)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.Notify, this))
            {
                if (@event.Name == "OnNewCreateMeasTaskEvent")
                {
                    
                }
                else if (@event.Name == "OnDelCreateMeasTaskEvent")
                {

                }
                else if (@event.Name == "OnStopCreateMeasTaskEvent")
                {

                }
            }
        }
    }
}
