using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class ExampleProcess : ProcessBase
    {
        public int TaskId;

        public ExampleProcess() : base("Example process")
        {
        }
    }
}
