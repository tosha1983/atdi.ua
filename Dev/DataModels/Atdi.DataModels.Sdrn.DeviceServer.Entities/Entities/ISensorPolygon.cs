using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ISensorPolygon_PK
    {
        int? Id { get; set; }
    }

    [Entity]
    public interface ISensorPolygon : ISensorPolygon_PK
    {
        int? SensorId { get; set; }
        double? Lon { get; set; }
        double? Lat { get; set; }
        ISensor SENSOR { get; set; }
    }
}
