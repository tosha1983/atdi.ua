﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public enum MeasType
    {
        SpectrumOccupation,
        Level,
        Offset,
        Frequency,
        FreqModulation,
        AmplModulation,
        BandwidthMeas,
        Bearing,
        SubAudioTone,
        Program,
        PICode,
        SoundID,
        Location,
        MonitoringStations,
        Signaling,
        IQReceive,
        Timetimestamp
    }
}