using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ILinkResSensor_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ILinkResSensor: ILinkResSensor_PK
    {
        long? SensorId { get; set; }
        long? ResMeasStaId { get; set; }
        ISensor SENSOR { get; set; }
        IResMeasStation RESMEASSTA { get; set; }
    }
}
