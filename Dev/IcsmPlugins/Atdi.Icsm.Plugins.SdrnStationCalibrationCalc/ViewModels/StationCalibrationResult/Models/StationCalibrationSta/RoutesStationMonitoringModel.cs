using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationResult
{
    public class RoutesStationMonitoringModel
    {
        public long Id { get; set; }
        public double? Altitude_m { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
