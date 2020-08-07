﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public class RefSpectrumParameters
    {
        public float PowerThreshold_dBm;

        public long[] StationIds;

        public string Comments;

        public string Projection;

        public string MapName;
    }
}
