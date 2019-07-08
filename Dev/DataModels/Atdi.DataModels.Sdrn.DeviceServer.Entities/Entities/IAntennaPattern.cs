using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IAntennaPattern_PK
    {
        int? Id { get; set; }
    }

    [Entity]
    public interface IAntennaPattern : IAntennaPattern_PK
    {
        int? SensorAntennaId { get; set; }
        double? Freq { get; set; }
        double? Gain { get; set; }
        string DiagA { get; set; }
        string DiagH { get; set; }
        string DiagV { get; set; }
        ISensorAntenna SENSORANTENNA { get; set; }
    }
}
