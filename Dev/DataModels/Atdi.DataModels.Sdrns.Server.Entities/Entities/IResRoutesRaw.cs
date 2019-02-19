using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IResRoutesRaw
    {
        int Id { get; set; }
        string RouteId { get; set; }
        double? Agl { get; set; }
        double? Asl { get; set; }
        DateTime? StartTime { get; set; }
        DateTime? FinishTime { get; set; }
        double? Lat { get; set; }
        double? Lon { get; set; }
        string PointStayType { get; set; }
        int? ResMeasId { get; set; }
        IResMeasRaw RESMEAS { get; set; }
    }
}
