using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Server
{
    public interface ISdrnEnvelope<TDeliveryObject>
    {
        string CorrelationToken { get; set; }

        string SensorName { get; set; }

        string SensorTechId { get; set; }

        TDeliveryObject DeliveryObject { get; set; }
    }
}
