using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands
{
    public class MesureTraceCommand: CommandBase<Parameters.MesureGpsLocationExampleParameter>
    {
        public MesureTraceCommand()
            : base(CommandType.MesureTrace, new MesureGpsLocationExampleParameter())
        {
        }

        public MesureTraceCommand(MesureGpsLocationExampleParameter parameter) 
            : base(CommandType.MesureTrace, parameter)
        {
        }

    }
}
