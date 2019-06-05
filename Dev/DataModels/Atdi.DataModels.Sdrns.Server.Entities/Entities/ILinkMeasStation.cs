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
        long Id { get; set; }
        long? MeasTaskId { get; set; }
        long? StationId { get; set; }
        IMeasTask MEASTASK { get; set; }
        IStation STATION { get; set; }
    }
}
