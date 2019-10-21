﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Icsm.CoverageEstimation.Models
{
    public class StandardConfig
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ProvinceConfig[] provincesConfig { get; set; }
    }
}
