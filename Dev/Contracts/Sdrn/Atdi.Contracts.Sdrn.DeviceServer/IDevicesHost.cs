using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface IDevicesHost : IDisposable
    {
        void Register(Type adapterType);
    }
}
