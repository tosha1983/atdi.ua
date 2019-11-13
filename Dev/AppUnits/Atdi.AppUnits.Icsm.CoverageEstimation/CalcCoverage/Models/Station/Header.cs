using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Icsm.CoverageEstimation.Models
{
    public class Header
    {
        public string Version { get; set; }
        public string Unicode { get; set; }
        public string Type { get; set; }
        public int CountStation { get; set; }
        public int CountMWS { get; set; }

    }
}
