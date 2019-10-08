using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Server.DevicesBus
{
    public enum MessageProcessingStatus
    {
        Created = 0,
        SentEvent = 1,
        Processing = 2,
        Processed = 3,
        Failure = 4
    }
    public interface IMessageProcessingScope<TDeliveryObject> : IDisposable
    {
        string SensorName { get; }

        string SensorTechId { get; }

        TDeliveryObject Delivery { get; }
        
        MessageProcessingStatus Status { get; set; }

        string ResultNote { get; set; }
    }
}
