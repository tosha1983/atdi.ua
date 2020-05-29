using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager
{
    public class IcsmMobStation
    {

        public DateTime CreatedDate { get; set; }

        public string ExternalCode { get; set; }

        public string ExternalSource { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string LicenseGsid { get; set; }

        public string RealGsid { get; set; }

        public string RegionCode { get; set; }

        public string Name { get; set; }

        public string CallSign { get; set; }


        public string StateName { get; set; }

        public string Standard { get; set; }

        public IcsmMobStationSite SITE { get; set; }

        public IcsmMobStationAntenna ANTENNA { get; set; }

        public IcsmMobStationTransmitter TRANSMITTER { get; set; }

        public IcsmMobStationReceiver RECEIVER { get; set; }

        public IcsmMobStationCoordinates COORDINATES { get; set; }

    }
}
