using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries
{
    public class ClientContextStationModelByParams
    {
        public long ClientContextId { get; set; }

        public string ExternalCode { get; set; }

        public string ExternalSource { get; set; }

        public string Standard { get; set; }
    }
}
