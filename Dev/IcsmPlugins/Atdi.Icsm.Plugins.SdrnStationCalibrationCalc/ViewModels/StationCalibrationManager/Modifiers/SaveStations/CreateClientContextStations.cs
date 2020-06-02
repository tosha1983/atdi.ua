using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Modifiers
{
    public class CreateClientContextStations
    {
        public long ClientContextId { get; set; }
        public IcsmMobStation[]  IcsmMobStation  { get; set; }
    }
}
