using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IMeasSubTask
    {
        long Id { get; set; }
        DateTime? TimeStart { get; set; }
        DateTime? TimeStop { get; set; }
        string Status { get; set; }
        int? Interval { get; set; }
        long? MeasTaskId { get; set; }
        IMeasTask MEASTASK { get; set; }
    }
}
