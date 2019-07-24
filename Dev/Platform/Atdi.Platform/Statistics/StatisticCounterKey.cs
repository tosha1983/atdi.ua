using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    class StatisticCounterKey : StatisticEntryKey<long>, IStatisticCounterKey
    {
        public StatisticCounterKey(string name, float scale = 1)
            : base(name)
        {
            this.Scale = scale;
        }

        public float Scale { get; }
    }
}
