using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Icsm.CoverageEstimation.Models
{
    public class DirectoryConfig
    {
        public string TempTIFFFilesDirectory { get; set; }
        public string MaskFilesICSTelecom { get; set; }
        public CommandsConfig CommandsConfig { get; set; }
        public string SpecifiedLogFile { get; set; }
        public string BinICSTelecomDirectory { get; set; }
        public string ICSTelecomProjectDirectory { get; set; }
        public string TempEwxFilesDirectory { get; set; }
        public string TemplateOutputFileNameForMobStation { get; set; }
        public string TemplateOutputFileNameForMobStation2 { get; set; }
    }
}
