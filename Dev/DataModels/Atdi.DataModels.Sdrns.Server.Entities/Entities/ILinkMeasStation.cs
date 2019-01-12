using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ILinkMeasStation
    {
        int Id { get; set; }
        int? MeasTaskId { get; set; }
        int? StationId { get; set; }
        IMeasTask MEASTASK { get; set; }
        IStation STATION { get; set; }
    }
}
