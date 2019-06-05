using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Results
{
    public class GpsResult : CommandResultPartBase
    {
        public GpsResult(ulong partIndex, CommandResultStatus status)
               : base(partIndex, status)
        {
        }
        public long? TimeCorrection { get; set; }
        public double? Lon;
        public double? Lat;
        public double? Asl;
        public Enums.DeviceStatus DeviceStatus;
    }
}
