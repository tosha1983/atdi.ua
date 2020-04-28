using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands
{
    public class EstimateRefLevelCommand : CommandBase<Parameters.EstimateRefLevelParameter>
    {
        public EstimateRefLevelCommand()
            : base(CommandType.MesureEstimateRefLevel, new EstimateRefLevelParameter())
        {
        }

        public EstimateRefLevelCommand(EstimateRefLevelParameter parameter)
            : base(CommandType.MesureEstimateRefLevel, parameter)
        {
        }
    }
}
