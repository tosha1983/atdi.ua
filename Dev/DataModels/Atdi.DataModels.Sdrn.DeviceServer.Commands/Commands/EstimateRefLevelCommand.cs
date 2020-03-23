using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Commands
{
    public class EstimateRefLevelCommand : CommandBase<Parameters.MesureTraceParameter>
    {
        public EstimateRefLevelCommand()
            : base(CommandType.MesureTrace, new MesureTraceParameter())
        {
        }

        public EstimateRefLevelCommand(MesureTraceParameter parameter)
            : base(CommandType.MesureTrace, parameter)
        {
        }
    }
}
