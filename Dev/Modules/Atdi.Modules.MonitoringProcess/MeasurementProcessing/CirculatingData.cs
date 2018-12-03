using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Modules.MonitoringProcess.Measurement;

namespace Atdi.Modules.MonitoringProcess
{
    public class CirculatingData
    {
        public int TaskId;
        public Emitting[] emittings;
        public ReferenceLevels referenceLevels;
    }
}
