using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{

    public interface ISpectrum_PK
    {
        int Id { get; set; }
 
    }

    [Entity]
    public interface ISpectrum : ISpectrum_PK
    {
        double? SpectrumStartFreq_MHz { get; set; }
        double? SpectrumSteps_kHz { get; set; }
        int? T1 { get; set; }
        int? T2 { get; set; }
        int? MarkerIndex { get; set; }
        double? Bandwidth_kHz { get; set; }
        int? CorrectnessEstimations { get; set; }
        int? Contravention { get; set; }
        int? TraceCount { get; set; }
        float? SignalLevel_dBm { get; set; }
        int? EmittingId { get; set; }
        byte[] LevelsdBm { get; set; }
        IEmitting EMITTING { get; set; }
    }
}

