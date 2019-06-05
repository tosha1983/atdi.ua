﻿using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IResStLevelCar
    {
        long Id { get; set; }
        long? ResStationId { get; set; }
        double? Bw { get; set; }
        double? Altitude { get; set; }
        double? CentralFrequency { get; set; }
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
