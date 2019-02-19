using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IResLocSensorRaw
    {
        int Id { get; set; }
        double? Lon { get; set; }
        double? Lat { get; set; }
        double? Asl { get; set; }
        double? Agl { get; set; }
        int? ResMeasId { get; set; }
        IResMeasRaw RESMEAS { get; set; }
    }
}
