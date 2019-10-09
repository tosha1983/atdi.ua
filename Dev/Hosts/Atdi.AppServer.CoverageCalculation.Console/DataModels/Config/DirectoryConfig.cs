using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.DataModels.CoverageCalculation
{
    public class DirectoryConfig
    {
        public string NameEwxFile { get; set; }
        public string TempTIFFFilesDirectory { get; set; }
        public string BlankTIFFFile { get; set; }
        public string MaskFilesICSTelecom { get; set; }
        public CommandsConfig CommandsConfig { get; set; }
        public string BinICSTelecomDirectory { get; set; }
        public string ICSTelecomProjectDirectory { get; set; }
        
    }
}
