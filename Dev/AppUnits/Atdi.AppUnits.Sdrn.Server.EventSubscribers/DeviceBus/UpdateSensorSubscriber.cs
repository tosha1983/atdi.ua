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
    [SubscriptionEvent(EventName = "OnUpdateSensorDeviceBusEvent", SubscriberName = "UpdateSensorSubscriber")]
    public class UpdateSensorSubscriber : SubscriberBase<DM.Sensor>
    {
        public UpdateSensorSubscriber(IMessagesSite messagesSite, ILogger logger) : base(messagesSite, logger)
        {
        }

        protected override void Handle(string sensorName, string sensorTechId, DM.Sensor deliveryObject)
        {
            throw new NotImplementedException($"Not updated sensor {deliveryObject.Name}");
        }
    }
}
