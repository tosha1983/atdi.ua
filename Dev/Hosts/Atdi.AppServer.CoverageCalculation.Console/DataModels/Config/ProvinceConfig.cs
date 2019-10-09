using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.DataModels.CoverageCalculation
{
    public class ProvinceConfig
    {
        public string Name { get; set; }
        public string ICSTelecomProjectFile { get; set; }
        public string OutTIFFFilesDirectory { get; set; }
        public CodeOperatorConfig[] CodeOperatorConfig { get; set; }
    }
}
