using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration
{
    public class SensorModel
    {
        public long Id { get; set; }
        public long? SensorIdentifierId { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public DateTime? BiuseDate { get; set; }
        public DateTime? EouseDate { get; set; }
        public double? Azimuth { get; set; }
        public double? Elevation { get; set; }
        public double? Agl { get; set; }
        public double? RxLoss { get; set; }
        public string TechId { get; set; }
    }
}
