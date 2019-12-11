using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSFPL.Enums
{
    public enum PowerRegister : int
    {
        Normal = 0,
        RFOverload = 1,
        IFOverload = 4,
    }
}
