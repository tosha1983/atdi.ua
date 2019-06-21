using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    public interface ISensorEquipment_PK
    {
        int? Id { get; set; }
    }

    [Entity]
    public interface ISensorEquipment : ISensorEquipment_PK
    {
        int? SensorId { get; set; }
        string Code { get; set; }
        string Manufacturer { get; set; }
        string Name { get; set; }
        string Family { get; set; }
        string TechId { get; set; }
        string Version { get; set; }
        double? LowerFreq { get; set; }
        double? UpperFreq { get; set; }
        double? RbwMin { get; set; }
        double? RbwMax { get; set; }
        double? VbwMin { get; set; }
        double? VbwMax { get; set; }
        bool? Mobility { get; set; }
        double? FftPointMax { get; set; }
        double? RefLevelDbm { get; set; }
        string OperationMode { get; set; }
        string Type { get; set; }
        string EquipClass { get; set; }
        double? TuningStep { get; set; }
        string UserType { get; set; }
        string Category { get; set; }
        string Remark { get; set; }
        string CustTxt1 { get; set; }
        DateTime? CustData1 { get; set; }
        double? CustNbr1 { get; set; }
        ISensor SENSOR { get; set; }
    }
}
