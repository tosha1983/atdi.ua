﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService
{
    [Serializable]
    public struct DiffractionLossResult
    {
        public double DiffractionLoss_dB;
        public int[] ObstaclesProfileIndexes;
    }
}