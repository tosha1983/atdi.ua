using Atdi.Contracts.CoreServices.Monitoring;
using Atdi.CoreServices.Monitoring.Statistics;
using Atdi.Platform.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.Monitoring
{
    class StatisticCounterKeys : IStatisticCounterKeys
    {
        private readonly StatisticCollector _collector;
        private readonly ILogger _logger;

        public StatisticCounterKeys(StatisticCollector collector, ILogger logger)
        {
            this._collector = collector;
            this._logger = logger;
        }
        public IEnumerator GetEnumerator()
        {
            var entries = _collector.GetCounterKeys();
            return entries.GetEnumerator();
        }
    }
}
