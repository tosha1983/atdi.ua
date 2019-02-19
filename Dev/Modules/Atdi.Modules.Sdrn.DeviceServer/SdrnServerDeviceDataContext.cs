using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.DeviceServer
{
    public class SdrnServerDeviceDataContext : DataContextBase
    {
        public SdrnServerDeviceDataContext() 
            : base("SDRN_DeviceServer_DB")
        {
        }
    }
}
