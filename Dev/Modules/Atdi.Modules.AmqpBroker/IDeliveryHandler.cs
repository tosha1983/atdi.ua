using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.AmqpBroker
{
    public enum HandlingResult
    {
        Ignore,
        Confirm,
        Reject
    }

    

    public interface IDeliveryHandler
    {
        HandlingResult Handle(IDeliveryMessage message, IDeliveryContext deliveryContext);
    }
}
