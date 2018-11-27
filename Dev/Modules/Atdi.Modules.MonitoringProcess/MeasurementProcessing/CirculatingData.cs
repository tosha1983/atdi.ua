using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.SDR.Server.MeasurementProcessing.Measurement;

namespace Atdi.SDR.Server.MeasurementProcessing
{
    public class CirculatingData
    {
        public int TaskId;
        public Emitting[] emittings;
        public ReferenceLevels referenceLevels;
    }
}
