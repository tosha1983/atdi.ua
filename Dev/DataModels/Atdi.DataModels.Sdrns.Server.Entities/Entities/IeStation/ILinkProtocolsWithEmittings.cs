using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities.IeStation
{
    [EntityPrimaryKeyAttribute]
    public interface ILinkProtocolsWithEmittings_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ILinkProtocolsWithEmittings : ILinkProtocolsWithEmittings_PK
    {
        double? Probability { get; set; }
        double? StartFrequency_MHz { get; set; }
        double? StopFrequency_MHz { get; set; }
        double? CurentPower_dBm { get; set; }
        double? ReferenceLevel_dBm { get; set; }
        double? MeanDeviationFromReference { get; set; }
        double? TriggerDeviationFromReference { get; set; }
        double? RollOffFactor { get; set; }
        double? StandardBW { get; set; }
        int[] LevelsDistributionLvl { get; set; }
        int[] LevelsDistributionCount { get; set; }
        float[] Loss_dB { get; set; }
        double[] Freq_kHz { get; set; }
        double? SpectrumStartFreq_MHz { get; set; }
        double? SpectrumSteps_kHz { get; set; }
        int? T1 { get; set; }
        int? T2 { get; set; }
        int? MarkerIndex { get; set; }
        double? Bandwidth_kHz { get; set; }
        bool CorrectnessEstimations { get; set; }
        bool Contravention { get; set; }
        int? TraceCount { get; set; }
        float? SignalLevel_dBm { get; set; }
        float[] Levels_dBm { get; set; }
        DateTime? WorkTimeStart { get; set; }
        DateTime? WorkTimeStop { get; set; }
        IProtocols PROTOCOLS { get; set; }
    }
}
