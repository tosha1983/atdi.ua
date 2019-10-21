using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Icsm.CoverageEstimation.Models
{
    public class FreqConfig
    {
        public string Values { get; set; }
        public ProvinceConfig[] provincesConfig { get; set; }
    }
}
