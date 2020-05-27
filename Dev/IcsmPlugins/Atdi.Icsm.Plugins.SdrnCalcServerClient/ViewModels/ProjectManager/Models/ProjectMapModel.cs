using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager
{
    public class ProjectMapModel
    {
        public long Id { get; set; }
        public string MapName { get; set; }
        public string MapNote { get; set; }
        public string OwnerInstance { get; set; }
        public Guid OwnerMapId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public byte StatusCode { get; set; }
        public string StatusName { get; set; }
        public string StatusNote { get; set; }
        public string StepUnit { get; set; }
        public int? OwnerAxisXNumber { get; set; }
        public int? OwnerAxisXStep { get; set; }
        public int? OwnerAxisYNumber { get; set; }
        public int? OwnerAxisYStep { get; set; }
        public int? OwnerUpperLeftX { get; set; }
        public int? OwnerUpperLeftY { get; set; }
    }
}
