using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands
{
    public class SetRotatorPositionCommand : CommandBase<Parameters.RotatorPositionParameter>
    {
        public SetRotatorPositionCommand()
            : base(CommandType.SetRotatorPosition, new RotatorPositionParameter())
        {
        }

        public SetRotatorPositionCommand(RotatorPositionParameter parameter)
            : base(CommandType.SetRotatorPosition, parameter)
        {
        }
    }
}
