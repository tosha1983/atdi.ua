using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IResLocSensorMeas_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IResLocSensorMeas : IResLocSensorMeas_PK
    {
        double? Lon { get; set; }
        double? Lat { get; set; }
        double? Asl { get; set; }
        double? Agl { get; set; }
        long? ResMeasId { get; set; }
        IResMeas RESMEAS { get; set; }
    }
}
