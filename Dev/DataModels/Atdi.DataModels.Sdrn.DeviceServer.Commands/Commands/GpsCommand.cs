using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands
{
    public class GpsCommand : CommandBase<Parameters.GpsParameter>
    {
        public GpsCommand()
            : base(CommandType.Gps, new GpsParameter())
        {
        }

        public GpsCommand(GpsParameter parameter) 
            : base(CommandType.Gps, parameter)
        {
        }

    }
}
