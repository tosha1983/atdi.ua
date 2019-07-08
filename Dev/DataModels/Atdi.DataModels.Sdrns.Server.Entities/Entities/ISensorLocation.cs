using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ISensorLocation_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ISensorLocation : ISensorLocation_PK
    {
        DateTime? DateFrom { get; set; }
        DateTime? DateTo { get; set; }
        DateTime? DateCreated { get; set; }
        string Status { get; set; }
        double? Lon { get; set; }
        double? Lat { get; set; }
        double? Asl { get; set; }
        ISensor SENSOR { get; set; }
    }
}
