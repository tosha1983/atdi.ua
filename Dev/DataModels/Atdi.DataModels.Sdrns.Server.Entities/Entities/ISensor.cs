using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ISensor
    {
        long Id { get; set; }
        long? SensorIdentifierId { get; set; }
        string Status { get; set; }
        string Name { get; set; }
        string Administration { get; set; }
        string NetworkId { get; set; }
        string Remark { get; set; }
        DateTime? BiuseDate { get; set; }
        DateTime? EouseDate { get; set; }
        double? Azimuth { get; set; }
        double? Elevation { get; set; }
        double? Agl { get; set; }
        string IdSysArgus { get; set; }
        string TypeSensor { get; set; }
        double? StepMeasTime { get; set; }
        double? RxLoss { get; set; }
        double? OpHhFr { get; set; }
        double? OpHhTo { get; set; }
        double? OpDays { get; set; }
        string CustTxt1 { get; set; }
        DateTime? CustData1 { get; set; }
        double? CustNbr1 { get; set; }
        DateTime? DateCreated { get; set; }
        string CreatedBy { get; set; }
        string ApiVersion { get; set; }
        string TechId { get; set; }
        DateTime? LastActivity { get; set; }
    }
}
