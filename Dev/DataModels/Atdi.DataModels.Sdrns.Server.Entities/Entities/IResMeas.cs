using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IResMeas_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IResMeas : IResMeas_PK
    {
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
        IMeasSubTaskStation MEAS_SUBTASK_STATION { get; set; }
    }
}
