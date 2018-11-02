using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IMeasSubTaskSta
    {
        int Id { get; set; }
        string Status { get; set; }
        int? Count { get; set; }
        DateTime? TimeNextTask { get; set; }
        int? SensorId { get; set; }
        int? MeasSubTaskId { get; set; }
        ISector SENSOR { get; set; }
        IMeasSubTask MEASSUBTASK { get; set; }
    }
}
