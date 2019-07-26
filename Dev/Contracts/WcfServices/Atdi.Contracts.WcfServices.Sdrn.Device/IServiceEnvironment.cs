using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.WcfServices.Sdrn.Device
{
    public interface IServiceEnvironment
    {
        string Instance { get; }

        string SdrnServerInstance { get; }

        string LicenseNumber { get; }

        DateTime LicenseStopDate { get; }

        DateTime LicenseStartDate { get; }

        IDictionary<string, string> AllowedSensors { get; }
    }
}
