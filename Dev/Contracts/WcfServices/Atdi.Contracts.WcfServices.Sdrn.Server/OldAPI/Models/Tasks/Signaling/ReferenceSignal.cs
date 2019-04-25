﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    public class ReferenceSignal
    {
        public double Frequency_MHz;
        public double Bandwidth_kHz;
        public double LevelSignal_dBm;
        public SignalMask SignalMask;
        public int IcsmId;
        public string IcsmTable;
    }
}
