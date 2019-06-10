﻿using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IEmitting
    {
        long Id { get; set; }
        double? StartFrequency_MHz { get; set; }
        double? StopFrequency_MHz { get; set; }
        double? CurentPower_dBm { get; set; }
        double? ReferenceLevel_dBm { get; set; }
        double? MeanDeviationFromReference { get; set; }
        double? TriggerDeviationFromReference { get; set; }
        double? RollOffFactor { get; set; }
        double? StandardBW { get; set; }
        long? ResMeasId { get; set; }
        //byte[] LevelsDistribution { get; set; }
        int[] LevelsDistributionLvl { get; set; }
        int[] LevelsDistributionCount { get; set; }
        IResMeas RESMEAS { get; set; }
        long? SensorId { get; set; }
        long? StationID { get; set; }
        string StationTableName { get; set; }
    }
}