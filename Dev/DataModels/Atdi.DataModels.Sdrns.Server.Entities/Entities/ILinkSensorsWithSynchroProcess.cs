using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ILinkSensorsWithSynchroProcess_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ILinkSensorsWithSynchroProcess : ILinkSensorsWithSynchroProcess_PK
    {
        long SensorId { get; set; }
        ISynchroProcess SYNCHRO_PROCESS { get; set; }
    }

}
