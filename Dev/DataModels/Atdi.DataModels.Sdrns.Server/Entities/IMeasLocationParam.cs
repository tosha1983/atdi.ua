using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IMeasLocationParam
    {
        int Id { get; set; }
        double? Lon { get; set; }
        double? Lat { get; set; }
        double? Asl { get; set; }
        double? MaxDist { get; set; }
        int? MeasTaskId { get; set; }
        IMeasTask MEASTASK { get; set; }
    }
}
