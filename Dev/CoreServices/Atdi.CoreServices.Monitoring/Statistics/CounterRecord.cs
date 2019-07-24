using Atdi.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.Monitoring.Statistics
{
    public struct CounterRecord
    {
        public string Name { get; set; }

        public DateTime Time { get; set; }

        public long Data { get; set; }

    }
}
