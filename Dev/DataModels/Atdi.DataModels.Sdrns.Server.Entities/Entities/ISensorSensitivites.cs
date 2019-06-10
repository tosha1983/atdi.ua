using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ISensorSensitivites
    {
        long Id { get; set; }
        long? SensorEquipId { get; set; }
        double? Freq { get; set; }
        double? Ktbf { get; set; }
        double? Noisef { get; set; }
        double? FreqStability { get; set; }
        double? AddLoss { get; set; }
        ISensorEquipment SENSOREQUIP { get; set; }
    }
}
