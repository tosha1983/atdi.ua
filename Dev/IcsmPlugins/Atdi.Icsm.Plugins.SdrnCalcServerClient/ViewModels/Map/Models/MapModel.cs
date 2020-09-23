using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.Map
{
    public class MapModel : ICloneable
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string MapName { get; set; }
        public string MapNote { get; set; }
        public Guid OwnerId { get; set; }
        public string StepUnit { get; set; }
        public int? OwnerAxisXNumber { get; set; }
        public int? OwnerAxisXStep { get; set; }
        public int? OwnerAxisYNumber { get; set; }
        public int? OwnerAxisYStep { get; set; }
        public int? OwnerUpperLeftX { get; set; }
        public int? OwnerUpperLeftY { get; set; }

        public object Clone()
        {
            return new MapModel()
            {
                Id = this.Id,
                MapName = this.MapName,
                MapNote = this.MapNote,
                OwnerAxisXNumber = this.OwnerAxisXNumber,
                OwnerAxisXStep = this.OwnerAxisXStep,
                OwnerAxisYNumber = this.OwnerAxisYNumber,
                OwnerAxisYStep = this.OwnerAxisYStep,
                OwnerUpperLeftX = this.OwnerUpperLeftX,
                OwnerUpperLeftY = this.OwnerUpperLeftY,
                OwnerId = this.OwnerId,
                ProjectId = this.ProjectId,
                StepUnit = this.StepUnit
            };
        }
    }
}
