using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Icsm.CoverageEstimation.Models
{
    public class CodeOperatorAndStatusConfig
    {
        public FreqConfig  FreqConfig { get; set; }
        public StandardConfig StandardConfig { get; set; }
        public string Status { get; set; }
        public string MaskFieldName { get; set; }
        public string MaskPattern { get; set; }
    }
}
