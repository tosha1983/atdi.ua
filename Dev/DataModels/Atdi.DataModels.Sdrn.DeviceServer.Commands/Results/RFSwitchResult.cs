using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Results
{
    [Serializable]
    public class RFSwitchResult : CommandResultPartBase
    {
        public RFSwitchResult(ulong partIndex, CommandResultStatus status)
                 : base(partIndex, status)
        { }
        public RFSwitchResult()
            : base()
        { }
        public bool ResultState;

    }
}
