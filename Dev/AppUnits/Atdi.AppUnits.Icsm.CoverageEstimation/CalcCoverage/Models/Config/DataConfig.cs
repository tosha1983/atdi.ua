﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Icsm.CoverageEstimation.Models
{
    public class DataConfig
    {
        //public CodeOperatorAndStatusConfig[] CodeOperatorAndStatusesConfig { get; set; }
        public BlockStationsConfig BlockStationsConfig { get; set; }
        public ColorConfig[] ColorsConfig { get; set; }
        public ProvinceCodeConfig[] ProvinceCodeConfig { get; set; }
        public DirectoryConfig DirectoryConfig { get; set; }
        public AnotherParameters AnotherParameters { get; set; }
}

}
