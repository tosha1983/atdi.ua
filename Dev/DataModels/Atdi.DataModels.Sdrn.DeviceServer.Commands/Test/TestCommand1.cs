using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands
{
    public class Test1CommandParameter
    {
        public int Count;
        public int Count2;
        public int Delay;
        public int ResultDelay;
        public double Property2;
        public float Property3;
    }

    public class Adapter1Result : CommandResultPartBase
    {
        public float Value;

        public Adapter1Result(ulong partIndex, CommandResultStatus status) : base(partIndex, status)
        {
        }
    }

    public class Test1CommandResult : CommandResultPartBase
    {
        public double Value;

        public Test1CommandResult(ulong partIndex, CommandResultStatus status) : base(partIndex, status)
        {
        }
    }

    public class Test1Command : CommandBase<Test1CommandParameter>
    {
        public Test1Command()
            : base(CommandType.MesureAudio, new Test1CommandParameter())
        {
        }

        public Test1Command(Test1CommandParameter parameter)
            : base(CommandType.MesureAudio, parameter)
        {
        }
    }
}
