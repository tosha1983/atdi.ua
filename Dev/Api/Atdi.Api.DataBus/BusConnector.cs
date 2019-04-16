using Atdi.Contracts.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.DataBus
{
    public static class BusConnector
    {
        private static readonly IBusGateFactory _busGateFactory = new BusGateFactory();

        public static IBusGateFactory GateFactory
        {
            get
            {
                return _busGateFactory;
            }
        }
    }
}
