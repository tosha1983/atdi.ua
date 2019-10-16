using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Icsm.CoverageEstimation.Models
{
    public class ProvinceConfig
    {
        public string Name { get; set; }
        public string ICSTelecomProjectFile { get; set; }
        public string OutTIFFFilesDirectory { get; set; }
        public string BlankTIFFFile { get; set; }
        public string NameEwxFile { get; set; }
        public CodeOperatorConfig[] CodeOperatorConfig { get; set; }
    }
}
