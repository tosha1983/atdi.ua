using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atdi.DataModels.CoverageCalculation
{
    public class DataConfig
    {
        public CodeOperatorAndStatusConfig[] CodeOperatorAndStatusesConfig { get; set; }
        public ColorConfig[] ColorsConfig { get; set; }
        public DirectoryConfig DirectoryConfig { get; set; }
        public WebQueryWcfServicesConfig WebQueryWcfServicesConfig { get; set; }
        public IdentityWcfServicesBindingConfig IdentityWcfServicesBindingConfig { get; set; }
    }

}
