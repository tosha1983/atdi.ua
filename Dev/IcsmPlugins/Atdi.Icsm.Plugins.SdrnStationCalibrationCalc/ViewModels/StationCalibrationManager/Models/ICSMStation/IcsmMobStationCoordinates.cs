using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager
{
    public class IcsmMobStationCoordinates
    {
        public int AtdiX { get; set; }
        public int AtdiY { get; set; }
        public double EpsgX { get; set; }
        public double EpsgY { get; set; }
    }
}
