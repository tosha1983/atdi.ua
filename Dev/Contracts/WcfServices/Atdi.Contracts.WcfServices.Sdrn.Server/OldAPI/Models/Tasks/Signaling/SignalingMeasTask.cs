using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    public class SignalingMeasTask
    {
        public bool? CompareTraceJustWithRefLevels { get; set; }
        public bool? AutoDivisionEmitting { get; set; }
        public double? DifferenceMaxMax { get; set; }
        public bool? FiltrationTrace { get; set; }
        public double? allowableExcess_dB { get; set; }
        public int? SignalizationNCount { get; set; }
        public int? SignalizationNChenal { get; set; }
    }
}
