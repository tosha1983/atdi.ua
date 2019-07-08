using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IReferenceSituation_PK
    {
        long? Id { get; set; }
    }

    [Entity]
    public interface IReferenceSituation : IReferenceSituation_PK
    {
        int? SensorId { get; set; }
        long? MeasTaskId { get; set; }
        IMeasTask MEASTASK { get; set; }
        ISensor SENSOR { get; set; }
    }
}

