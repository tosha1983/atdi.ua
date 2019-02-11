using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Adapters.Enums
{
    public enum LevelUnit : uint
    {
        dBm = 2,
        dBmV = 4,
        dBµV = 8,
        dBµVm = 16,
        mV = 32,
        µV = 64,
    }
}
