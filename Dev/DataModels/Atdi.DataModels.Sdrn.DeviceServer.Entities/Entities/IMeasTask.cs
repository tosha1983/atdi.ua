using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [Entity]
    public interface IMeasTask
    {
        int Id { get; set; }
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
