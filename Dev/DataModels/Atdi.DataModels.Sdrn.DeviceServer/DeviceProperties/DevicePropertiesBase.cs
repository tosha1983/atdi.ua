using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public interface IDeviceProperties
    {
        Guid DeviceId { get; set; }
    }

    public class DevicePropertiesBase : IDeviceProperties
    {
        public Guid DeviceId { get; set; }
    }
}
