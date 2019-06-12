using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results.MesureSystemInfo;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Results
{
    [Serializable]
    public class MesureSystemInfoResult : CommandResultPartBase
    {
        public MesureSystemInfoResult(ulong partIndex, CommandResultStatus status)
               : base(partIndex, status)
        {
        }
        public StationSystemInfo[] SystemInfo;
        public Enums.DeviceStatus DeviceStatus;
    }
}
