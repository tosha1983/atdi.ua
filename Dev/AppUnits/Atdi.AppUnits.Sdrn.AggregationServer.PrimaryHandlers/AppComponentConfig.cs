using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers
{
    public class AppComponentConfig
    {
        [ComponentConfigProperty("MeasResultSignalizationWorker.Timeout")]
        public int? MeasResultSignalizationWorkerTimeout { get; set; }

        [ComponentConfigProperty("MeasResultSignalizationWorker.QtyDayAwaitNextResult")]
        public int? MeasResultSignalizationWorkerQtyDayAwaitNextResult { get; set; }
    }
}
