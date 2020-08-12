using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager
{
    public class AreaModel
    {
        public int IdentifierFromICSM { get; set; }

        public string Name { get; set; }

        public string TypeArea { get; set; }

        public string CSys { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? DateCreated { get; set; }

        public DataLocationModel[] Location { get; set; }

        public DataLocationModel[] ExternalContour { get; set; }
    }
}
