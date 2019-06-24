using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IMeasStation_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IMeasStation: IMeasStation_PK
    {
        int? StationId { get; set; }
        string StationType { get; set; }
        long? MeasTaskId { get; set; }
        IStation STATION { get; set; }
        IMeasTask MEASTASK { get; set; }
    }
}
