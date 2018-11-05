using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IMeasTaskSDR
    {
        int Id { get; set; }
        int? MeasTaskId { get; set; }
        int? MeasSubTaskId { get; set; }
        int? MeasSubTaskStaId { get; set; }
        int? Num { get; set; }
        int? SensorId { get; set; }
        IMeasTask MEASTASK { get; set; }
        IMeasSubTask MEASSUBTASK { get; set; }
        IMeasSubTaskSta MEASSUBTASKSTA { get; set; }
        ISensor SENSOR { get; set; }
    }
}
