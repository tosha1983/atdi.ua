using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing.Test
{
    public class Test2Process : ProcessBase
    {
        public Test2Process()
            : base("Test 2 Process")
        {
            this.TotalValues = new List<double>();
        }

        public bool IsDone;

        public List<double> TotalValues;

    }
}
