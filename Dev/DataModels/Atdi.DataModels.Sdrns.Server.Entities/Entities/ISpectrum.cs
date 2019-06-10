﻿using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ISpectrum
    {
        long Id { get; set; }
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
        long? EmittingId { get; set; }
        byte[] LevelsdBm { get; set; }
        IEmitting EMITTING { get; set; }
    }
}

