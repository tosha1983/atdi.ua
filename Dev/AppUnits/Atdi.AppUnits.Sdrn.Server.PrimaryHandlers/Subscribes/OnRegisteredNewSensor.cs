using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes
{
    [SubscriptionEvent("OnRegisteredNewSensor")]
    [SubscriptionEvent(EventName = "OnRegisteredNewSensor2", SubscriberName = "Subscriber1")]
    public class OnRegisteredNewSensor : IEventSubscriber<Event>
    {
        private readonly ILogger _logger;

        public OnRegisteredNewSensor(ILogger logger)
        {
            this._logger = logger;
        }

        public void Notify(Event @event)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.Notify, this))
            {
                if (@event.Name == "OnRegisteredNewSensor")
                {

                }
                if(@event.Name == "OnRegisteredNewSensor2")
                {

                }
            }
        }
    }
}
