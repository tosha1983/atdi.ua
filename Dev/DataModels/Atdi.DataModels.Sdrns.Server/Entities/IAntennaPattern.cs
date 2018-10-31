using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IAntennaPattern
    {
        int Id { get; set; }
        int? SensorAntennaId { get; set; }
        double? Freq { get; set; }
        double? Gain { get; set; }
        string DiagA { get; set; }
        string DiagV { get; set; }
        ISensorAntenna SENSORANTENNA { get; set; }
    }
}
