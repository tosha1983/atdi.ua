using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Adapters
{
    public class DeviceOption
    {
        public DeviceOption()
        {
        }
        public string Type { get; set; } = "";
        public string GlobalType { get; set; } = "";
        public string Designation { get; set; } = "";
    }
}
