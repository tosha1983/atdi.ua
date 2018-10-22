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

        int SensorAntennaId { get; set; }

        string Freq { get; set; }

        string Gain { get; set; }


        ISensorAntenna SENSORANT { get; set; }
    }
}
