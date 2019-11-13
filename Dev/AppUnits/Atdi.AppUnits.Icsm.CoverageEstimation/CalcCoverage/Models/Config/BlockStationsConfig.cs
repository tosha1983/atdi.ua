using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Icsm.CoverageEstimation.Models
{
    public class BlockStationsConfig
    {
        public CodeOperatorAndStatusConfig[] MobStationConfig { get; set; }
        public CodeOperatorAndStatusConfig[] MobStation2Config { get; set; }
    }

}
