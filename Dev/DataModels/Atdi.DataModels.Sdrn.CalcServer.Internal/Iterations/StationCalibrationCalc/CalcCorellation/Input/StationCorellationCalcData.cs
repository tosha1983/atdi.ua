﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
    [Serializable]
    public class StationCorellationCalcData: StationCorellationCalcDataBase
    {
        public CorellationParameters  CorellationParameters;
        public GeneralParameters GeneralParameters;

    }
}
