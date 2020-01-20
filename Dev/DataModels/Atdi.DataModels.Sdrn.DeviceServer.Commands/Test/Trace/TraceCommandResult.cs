using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.TestCommands
{
    public class TraceCommandResult: CommandResultPartBase
    {
        public TraceCommandResult() : base()
        {
        }
        public TraceCommandResult(ulong partIndex, CommandResultStatus status) : base(partIndex, status)
        {
        }

        public Guid Id;
        public float[] ValueAsFloats;
        public double[] ValuesAsDouble;
    }
}
