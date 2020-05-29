using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries
{
    public class MobStationsLoadModelByParams
    {
        public string Standard;
        public string StatusForActiveStation;
        public string StatusForNotActiveStation;
        public int? IdentifierStation;
        public string TableName;
        public AreaModel[] AreaModel;
        public SelectedStationType  SelectedStationType;
    }
}
