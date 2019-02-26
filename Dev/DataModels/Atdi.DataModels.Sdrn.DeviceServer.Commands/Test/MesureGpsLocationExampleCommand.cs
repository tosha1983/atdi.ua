using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands
{
    public class MesureGpsLocationExampleCommand : CommandBase<Parameters.MesureGpsLocationExampleParameter>
    {
        public MesureGpsLocationExampleCommand()
            : base(CommandType.MesureGpsLocation, new MesureGpsLocationExampleParameter())
        {
        }

        public MesureGpsLocationExampleCommand(MesureGpsLocationExampleParameter parameter) 
            : base(CommandType.MesureGpsLocation, parameter)
        {
        }

    }
}
