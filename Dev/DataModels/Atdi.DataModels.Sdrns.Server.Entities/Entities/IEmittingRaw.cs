using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IEmittingRaw
    {
        int Id { get; set; }
        double? StartFrequency_MHz { get; set; }
        double? StopFrequency_MHz { get; set; }
        double? CurentPower_dBm { get; set; }
        double? ReferenceLevel_dBm { get; set; }
        double? MeanDeviationFromReference { get; set; }
        double? TriggerDeviationFromReference { get; set; }
        double? RollOffFactor { get; set; }
        double? StandardBW { get; set; }
        int? ResMeasId { get; set; }
        byte[] LevelsDistribution { get; set; }
        IResMeasRaw RESMEASRAW { get; set; }
        string SensorName { get; set; }
        string TechId { get; set; }
    }
}