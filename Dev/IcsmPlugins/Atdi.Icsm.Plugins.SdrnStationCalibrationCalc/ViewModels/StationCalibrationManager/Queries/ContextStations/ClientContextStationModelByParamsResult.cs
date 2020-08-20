using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries
{
    public class ClientContextStationModelByParamsResult
    {
        public long? StationId { get; set; }

        public DateTimeOffset? DateModified { get; set; }

        public string Status { get; set; }
        
    }
}
