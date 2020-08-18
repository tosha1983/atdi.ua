using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration
{
    public class PivotTableConfiguration
    {
        public bool IsUseDefaultThreshold { get; set; }
        public long Threshold { get; set; }
        public string Comments { get; set; }

        public string GSID { get; set; }
        public long CorrelationThreshold { get; set; }
        public string Status { get; set; }
    }
}
