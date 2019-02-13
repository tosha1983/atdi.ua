using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IResStLevelCarRaw
    {
        int Id { get; set; }
        int? ResStationId { get; set; }
        double? Bw { get; set; }
        double? Altitude { get; set; }
        double? CentralFrequency { get; set; }
        double? DifferenceTimeStamp { get; set; }
        double? Lon { get; set; }
        double? Lat { get; set; }
        double? LevelDbm { get; set; }
        double? LevelDbmkvm { get; set; }
        double? Rbw { get; set; }
        DateTime? TimeOfMeasurements { get; set; }
        double? Vbw { get; set; }
        double? Agl { get; set; }
        IResMeasStaRaw RESSTATION { get; set; }
    }
}
