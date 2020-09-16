using Atdi.WpfControls.EntityOrm.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration
{
    public class CalcTaskModel
    {
        public long Id { get; set; }
        public int TypeCode { get; set; }
        public string TypeName { get; set; }
        public byte StatusCode { get; set; }
        public string StatusName { get; set; }
        public string StatusNote { get; set; }
        public string OwnerInstance { get; set; }
        public Guid OwnerTaskId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string MapName { get; set; }
        public OrmEnumBoxData MapCombo { get; set; }
        public long ContextId { get; set; }
        public string ContextName { get; set; }
    }
}
