using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager
{
    public class IcsmMobStationAntenna
    {
        public float Gain_dB { get; set; }

        public double Tilt_deg { get; set; }

        public double Azimuth_deg { get; set; }

        public float XPD_dB { get; set; }

        public byte ItuPatternCode { get; set; }

        public string ItuPatternName { get; set; }

        IcsmMobStationPattern HH_PATTERN { get; set; }

        IcsmMobStationPattern HV_PATTERN { get; set; }

        IcsmMobStationPattern VH_PATTERN { get; set; }

        IcsmMobStationPattern VV_PATTERN { get; set; }

    }
}
