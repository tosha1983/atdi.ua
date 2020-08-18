﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public class RefSpectrumStationCalibration
    {
        public string RealGsid;
        public long StationMonitoringId;
        public double Old_Freq_MHz;
        public double Freq_MHz;
        public string Standard;
        public ContextStation contextStation;
    }
}