using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IMeasSubTask_PK
    {
        long Id { get; set; }
    }
    [Entity]
    public interface IMeasSubTask: IMeasSubTask_PK
    {
        DateTime? TimeStart { get; set; }
        DateTime? TimeStop { get; set; }
        string Status { get; set; }
        int? Interval { get; set; }
        IMeasTask MEAS_TASK { get; set; }
    }
}
