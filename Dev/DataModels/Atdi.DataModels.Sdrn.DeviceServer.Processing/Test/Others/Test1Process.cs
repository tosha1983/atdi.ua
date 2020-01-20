using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing.Test
{
    public class Test1Process : ProcessBase
    {
        public Test1Process()
            : base("Test 1 Process")
        {
            this.TotalValues = new List<double>();
        }

        public bool IsDone;

        public List<double> TotalValues;
    }
}
