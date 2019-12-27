using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeviceServer
{
    public interface IDeviceServerConfig
    {
        string SdrnServerInstance { get; }

        string SensorName { get; }

        string SensorTechId { get; }

        string LicenseNumber { get; }

        DateTime LicenseStopDate { get; }

        DateTime LicenseStartDate { get; }

        int? CommandContextPoolObjects { get; }
    }
}
