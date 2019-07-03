using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ISensorPolygon_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ISensorPolygon : ISensorPolygon_PK
    {
        double? Lon { get; set; }
        double? Lat { get; set; }
        ISensor SENSOR { get; set; }
    }
}
