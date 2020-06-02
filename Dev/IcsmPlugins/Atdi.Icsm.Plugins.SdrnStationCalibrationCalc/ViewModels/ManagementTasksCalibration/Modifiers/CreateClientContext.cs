using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration.Modifiers
{
    public class CreateClientContext
    {
        public long BaseContextId;

        public long ProjectId;

        public Guid OwnerId;

        public string Name;

        public string Note;

        public bool Success;
    }
}
