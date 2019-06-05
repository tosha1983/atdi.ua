using Atdi.DataModels.Api.EventSystem;
using DM = Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.Server.EventSubscribers.DeviceBus
{
    [SubscriptionEvent(EventName = "OnSendEntityDeviceBusEvent", SubscriberName = "SendEntitySubscriber")]
    public class SendEntitySubscriber : SubscriberBase<DM.Entity>
    {
        public SendEntitySubscriber(IMessagesSite messagesSite, ILogger logger) : base(messagesSite, logger)
        {
        }

        protected override void Handle(string sensorName, string sensorTechId, DM.Entity deliveryObject)
        {
            throw new NotImplementedException($"Not handled entity {deliveryObject.EntityId}");
        }
    }
}
