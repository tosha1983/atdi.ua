using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class ExampleTask : TaskBase
    {
        public int ExampleValue1;
        public int ExampleValue2;

        public MeasTask ServerMeasTask;
    }

    public class ExampleTask2 : ExampleTask
    {
        public int ExampleValue3;
    }
}
