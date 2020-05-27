using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager
{
    public class StationMonitoringModel
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public string SensorName { get; set; }
        public string SensorTitle { get; set; }
        public string Standards { get; set; }
        public int CountByStandard { get; set; }
        public int CountSID { get; set; }
        public double? MinFreq_MHz { get; set; }
        public double? MaxFreq_MHz { get; set; }
    }
}
