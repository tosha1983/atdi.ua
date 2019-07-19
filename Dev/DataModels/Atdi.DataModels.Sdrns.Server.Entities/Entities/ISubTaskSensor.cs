using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ISubTaskSensor_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ISubTaskSensor: ISubTaskSensor_PK
    {
        string Status { get; set; }
        int? Count { get; set; }
        DateTime? TimeNextTask { get; set; }
        ISensor SENSOR { get; set; }
        ISubTask SUBTASK { get; set; }
    }
}
