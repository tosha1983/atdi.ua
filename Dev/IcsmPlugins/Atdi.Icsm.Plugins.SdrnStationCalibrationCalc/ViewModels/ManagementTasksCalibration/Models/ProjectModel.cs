using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration
{
    public class ProjectModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string OwnerInstance { get; set; }
        public Guid OwnerProjectId { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public byte StatusCode { get; set; }
        public string StatusName { get; set; }
        public string StatusNote { get; set; }
        public string Projection { get; set; }
    }
}
