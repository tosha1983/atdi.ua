using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands
{
    public class MesureTraceCommand: CommandBase<Parameters.MesureTraceParameter>
    {
        public MesureTraceCommand()
            : base(CommandType.MesureTrace, new MesureTraceParameter())
        {
        }

        public MesureTraceCommand(MesureTraceParameter parameter) 
            : base(CommandType.MesureTrace, parameter)
        {
        }

    }
}
