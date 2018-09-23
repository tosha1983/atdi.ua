using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.Sdrn.Device.BusController
{
    internal class QueueDescriptor
    {
        public string Name;
        public string RoutingKey;
        public string Exchange;
    }

    internal class ExchangeDescriptor
    {
        public string RoutingKey;
        public string Exchange;
    }
}
