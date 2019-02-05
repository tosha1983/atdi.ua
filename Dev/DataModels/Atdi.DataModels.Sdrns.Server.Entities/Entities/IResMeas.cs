using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IResMeas
    {
        int Id { get; set; }
        string MeasTaskId { get; set; }
        int? MeasSubTaskId { get; set; }
        int? MeasSubTaskStationId { get; set; }
        int? SensorId { get; set; }
        double? AntVal { get; set; }
        DateTime? TimeMeas { get; set; }
        int? DataRank { get; set; }
        int? N { get; set; }
        string Status { get; set; }
        string MeasResultSID { get; set; }
        string TypeMeasurements { get; set; }
        bool Synchronized { get; set; }
        DateTime? StartTime { get; set; }
        DateTime? StopTime { get; set; }
        int? ScansNumber { get; set; }
        IMeasSubTask MEASSUBTASK { get; set; }
        IMeasSubTaskSta MEASSUBTASKSTA { get; set; }
        ISensor SENSOR { get; set; }
    }
}
