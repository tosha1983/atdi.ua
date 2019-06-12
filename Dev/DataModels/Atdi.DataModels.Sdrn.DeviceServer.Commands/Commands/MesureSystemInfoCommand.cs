using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Commands
{
    class MesureSystemInfoCommand : CommandBase<Parameters.MesureSystemInfoParameter>
    {
        public MesureSystemInfoCommand()
            : base(CommandType.MesureSysInfo, new MesureSystemInfoParameter())
        {
        }

        public MesureSystemInfoCommand(MesureSystemInfoParameter parameter)
            : base(CommandType.MesureSysInfo, parameter)
        {
        }
    }
}
