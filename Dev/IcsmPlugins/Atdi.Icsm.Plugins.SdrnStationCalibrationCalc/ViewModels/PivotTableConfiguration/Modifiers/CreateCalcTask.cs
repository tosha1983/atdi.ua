using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.PivotTableConfiguration.Modifiers
{
    public class CreateCalcTask
    {
        public string MapName;
        public long ContextId;
        public Guid OwnerId;
        public long ResultId;
        public long[] StationIds;
        public float PowerThreshold_dBm;
        public string Comments;
    }
}
