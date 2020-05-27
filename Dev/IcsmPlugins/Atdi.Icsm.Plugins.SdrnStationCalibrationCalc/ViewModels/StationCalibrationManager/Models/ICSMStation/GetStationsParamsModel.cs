using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager
{
    public class GetStationsParamsModel
    {
        public int Id { get; set; }
        public string StateForActiveStation { get; set; }
        public string StateForNotActiveStation { get; set; }
        public string Standard { get; set; }
    }
}
