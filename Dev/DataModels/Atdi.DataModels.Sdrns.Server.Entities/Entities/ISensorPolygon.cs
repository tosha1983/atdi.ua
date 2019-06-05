using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ISensorPolygon
    {
        long Id { get; set; }
        long? SensorId { get; set; }
        double? Lon { get; set; }
        double? Lat { get; set; }
        ISensor SENSOR { get; set; }
    }
}
