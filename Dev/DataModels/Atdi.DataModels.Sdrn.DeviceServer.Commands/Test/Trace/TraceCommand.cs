using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.TestCommands
{
    

    public class TraceCommand : CommandBase<TraceCommandParameter>
    {
        public TraceCommand() 
            : base(CommandType.MesureTrace, new TraceCommandParameter())
        {
        }

        public TraceCommand(TraceCommandParameter parameter)
            : base(CommandType.MesureTrace, parameter)
        {
        }

        public int BlockSize;

    }
}
