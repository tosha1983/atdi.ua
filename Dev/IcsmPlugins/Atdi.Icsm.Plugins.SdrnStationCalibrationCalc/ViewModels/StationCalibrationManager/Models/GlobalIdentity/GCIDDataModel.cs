using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager
{
    public class GCIDDataModel
    {
        public string RegionCode { get; set; }

        public string LicenseGsid { get; set; }

        public string Standard { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public string RealGsid { get; set; }
    }
}
