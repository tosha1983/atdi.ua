using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [Entity]
    public interface ISectorMaskElement
    {
        int Id { get; set; }
        double? Level { get; set; }
        double? Bw { get; set; }
        int? IdSector { get; set; }
    }
}
