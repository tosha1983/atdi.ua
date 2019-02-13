using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands
{
    public class MesureIQStreamCommand: CommandBase<Parameters.MesureIQStreamParameter>
    {
        public MesureIQStreamCommand()
            : base(CommandType.MesureIQStream, new MesureIQStreamParameter())
        {
        }

        public MesureIQStreamCommand(MesureIQStreamParameter parameter) 
            : base(CommandType.MesureIQStream, parameter)
        {
        }

    }
}
