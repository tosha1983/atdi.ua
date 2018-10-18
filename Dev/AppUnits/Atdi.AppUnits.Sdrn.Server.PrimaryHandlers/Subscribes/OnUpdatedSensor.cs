using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes
{
    [SubscriptionEvent(EventName = "OnEvent7", SubscriberName = "Subscriber7")]
    public class OnUpdatedSensor : IEventSubscriber<Event>
    {
        private readonly ILogger _logger;

        public OnUpdatedSensor(ILogger logger)
        {
            this._logger = logger;
        }

        public void Notify(Event @event)
        {
            using (this._logger.StartTrace(Contexts.PrimaryHandler, Categories.Notify, this))
            {

            }
        }
    }
}
