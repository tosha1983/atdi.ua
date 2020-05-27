using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.Map.Modifiers
{
    public class CreateMap
    {
        public long ProjectId;

        public string MapName;

        public string MapNote;

        public Guid OwnerId;

        public string StepUnit;

        public int OwnerAxisXNumber;

        public int OwnerAxisXStep;

        public int OwnerAxisYNumber;

        public int OwnerAxisYStep;

        public int OwnerUpperLeftX;

        public int OwnerUpperLeftY;
    }
}
