using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    public interface IMeasTask_PK
    {
        long Id { get; set; }
    }

        [Entity]
    public interface IMeasTask: IMeasTask_PK
    {
        string Status { get; set; }
        int? OrderId { get; set; }
        string Type { get; set; }
        string Name { get; set; }
        string ExecutionMode { get; set; }
        string Task { get; set; }
        int? Prio { get; set; }
        string ResultType { get; set; }
        int? MaxTimeBs { get; set; }
        DateTime? DateCreated { get; set; }
        string CreatedBy { get; set; }
        string IdStart { get; set; }
        DateTime? PerStart { get; set; }
        DateTime? PerStop { get; set; }
        DateTime? TimeStart { get; set; }
        DateTime? TimeStop { get; set; }
        double? PerInterval { get; set; }
    }
}
