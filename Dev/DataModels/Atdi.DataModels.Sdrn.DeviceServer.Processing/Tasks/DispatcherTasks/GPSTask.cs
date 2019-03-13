using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class GPSTask : TaskBase
    {
       public double? Lon { get; set; }
       public double? Lat { get; set; }
       public double? Asl { get; set; }
    }
}
