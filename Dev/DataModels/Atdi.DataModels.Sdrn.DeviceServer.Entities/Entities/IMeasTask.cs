using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IMeasTask_PK
    {
        long? Id { get; set; }
    }

    [Entity]
    public interface IMeasTask : IMeasTask_PK
    {
        string Status { get; set; }
        string TaskId { get; set; }
        string SdrnServer { get; set; }
        string SensorName { get; set; }
        string EquipmentTechId { get; set; }
        DateTime? TimeStart { get; set; }
        DateTime? TimeStop { get; set; }
        int? Priority { get; set; }
        int? ScanPerTaskNumber { get; set; }
        string MobEqipmentMeasurements { get; set; }
    }
}
