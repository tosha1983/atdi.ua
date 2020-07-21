using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands
{
    public class SetRFSwitchSettingsCommand : CommandBase<Parameters.RFSwitchParameter>
    {
        public SetRFSwitchSettingsCommand()
            : base(CommandType.SetRFSwitch, new RFSwitchParameter())
        {
        }

        public SetRFSwitchSettingsCommand(RFSwitchParameter parameter)
            : base(CommandType.SetRFSwitch, parameter)
        {
        }       
    }
}
