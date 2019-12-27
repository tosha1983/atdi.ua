using Atdi.Contracts.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer
{
    class DeviceServerConfig : IDeviceServerConfig
    {
        public string SdrnServerInstance { get; set; }

        public string SensorName { get; set; }

        public string SensorTechId { get; set; }

        public string LicenseNumber { get; set; }

        public DateTime LicenseStopDate { get; set; }

        public DateTime LicenseStartDate { get; set; }

        public int? CommandContextPoolObjects { get; set; }
    }
}
