using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    public interface ISensorSensitivites_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ISensorSensitivites : ISensorSensitivites_PK
    {
        long? SensorEquipId { get; set; }
        double? Freq { get; set; }
        double? Ktbf { get; set; }
        double? Noisef { get; set; }
        double? FreqStability { get; set; }
        double? AddLoss { get; set; }
        ISensorEquipment SENSOREQUIP { get; set; }
    }
}
