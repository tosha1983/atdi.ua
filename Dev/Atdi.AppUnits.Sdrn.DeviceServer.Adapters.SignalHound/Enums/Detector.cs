using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.EN
{
    public enum Detector : uint
    {
        MinAndMax = 0x0,
        Average = 0x1,
        MinOnly = 0x2,//Сейчас нету
        MaxOnly = 0x3,//Сейчас нету
    }
}
