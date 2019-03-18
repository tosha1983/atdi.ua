using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands
{
    public class TestCommand1Parameter
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

    public class TestCommand1Result : CommandResultPartBase
    {
        public double Value;

        public TestCommand1Result(ulong partIndex, CommandResultStatus status) : base(partIndex, status)
        {
        }
    }

    public class TestCommand1 : CommandBase<TestCommand1Parameter>
    {
        public TestCommand1()
            : base(CommandType.MesureAudio, new TestCommand1Parameter())
        {
        }

        public TestCommand1(TestCommand1Parameter parameter)
            : base(CommandType.MesureAudio, parameter)
        {
        }
    }
}
