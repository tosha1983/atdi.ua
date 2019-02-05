using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public enum CommandOption
    {
        StartImmediately = 0,
        StartDelayed = 1,
        StartSpecificTime = 3
    }
}
