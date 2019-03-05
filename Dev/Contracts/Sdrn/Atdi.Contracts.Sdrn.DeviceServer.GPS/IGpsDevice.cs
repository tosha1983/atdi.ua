using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer.GPS
{
    public interface IGpsDevice
    {
        void Run();
        void Stop();
    }
}
