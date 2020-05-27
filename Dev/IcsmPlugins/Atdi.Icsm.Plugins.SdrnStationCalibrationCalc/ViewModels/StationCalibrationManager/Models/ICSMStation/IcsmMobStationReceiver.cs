﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager
{
    public class IcsmMobStationReceiver
    {
        public double[] Freqs_MHz { get; set; }

        public double Freq_MHz { get; set; }

        public double BW_kHz { get; set; }

        public float Loss_dB { get; set; }

        public float KTBF_dBm { get; set; }

        public float Threshold_dBm { get; set; }

        public byte PolarizationCode { get; set; }

        public string PolarizationName { get; set; }

    }
}
