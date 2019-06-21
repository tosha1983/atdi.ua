using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    public interface IResRoutes_PK
    {
        long Id { get; set; }
    }


    [Entity]
    public interface IResRoutes : IResRoutes_PK
    {
        string RouteId { get; set; }
        double? Agl { get; set; }
        double? Asl { get; set; }
        DateTime? StartTime { get; set; }
        DateTime? FinishTime { get; set; }
        double? Lat { get; set; }
        double? Lon { get; set; }
        string PointStayType { get; set; }
        long? ResMeasId { get; set; }
        IResMeas RESMEAS { get; set; }
    }
}
