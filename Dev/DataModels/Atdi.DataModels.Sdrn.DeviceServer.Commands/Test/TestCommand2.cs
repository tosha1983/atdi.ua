using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands
{
    public class TestCommand2Parameter
    {
        public int Count;
        public int Predel;
    }

    public class Adapter2Result : CommandResultPartBase
    {
        public float Value;

        public Adapter2Result(ulong partIndex, CommandResultStatus status) : base(partIndex, status)
        {
        }
    }

    public class TestCommand2Result : CommandResultPartBase
    {
        public double Value;

        public TestCommand2Result(ulong partIndex, CommandResultStatus status) : base(partIndex, status)
        {
        }
    }

    public class TestCommand2 : CommandBase<TestCommand2Parameter>
    {
        public TestCommand2()
            : base(CommandType.MesureAudio, new TestCommand2Parameter())
        {
        }

        public TestCommand2(TestCommand2Parameter parameter)
            : base(CommandType.MesureAudio, parameter)
        {
        }
    }
}
