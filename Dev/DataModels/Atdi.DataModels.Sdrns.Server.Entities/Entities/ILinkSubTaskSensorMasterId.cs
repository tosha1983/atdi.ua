using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ILinkSubTaskSensorMasterId_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ILinkSubTaskSensorMasterId : ILinkSubTaskSensorMasterId_PK
    {
        ISubTaskSensor SUBTASK_SENSOR { get; set; }
        long? SubtaskSensorMasterId { get; set; }
    }
}
