using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IMeasSubTaskStation_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IMeasSubTaskStation: IMeasSubTaskStation_PK
    {
        string Status { get; set; }
        int? Count { get; set; }
        DateTime? TimeNextTask { get; set; }
        ISector SENSOR { get; set; }
        IMeasSubTask MEAS_SUB_TASK { get; set; }
    }
}
