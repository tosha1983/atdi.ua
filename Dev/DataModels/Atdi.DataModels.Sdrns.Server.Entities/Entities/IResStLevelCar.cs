using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IResStLevelCar_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IResStLevelCar : IResStLevelCar_PK
    {
        long? ResStationId { get; set; }
        double? Altitude { get; set; }
        double? DifferenceTimeStamp { get; set; }
        double? Lon { get; set; }
        double? Lat { get; set; }
        double? LevelDbm { get; set; }
        double? LevelDbmkvm { get; set; }
        DateTime? TimeOfMeasurements { get; set; }
        double? Agl { get; set; }
        IResMeasStation RESSTATION { get; set; }
    }
}
