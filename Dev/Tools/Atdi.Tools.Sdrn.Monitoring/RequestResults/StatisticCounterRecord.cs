using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Tools.Sdrn.Monitoring
{
    public class StatisticCounterRecord
    {
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public long Data { get; set; }
    }
}
