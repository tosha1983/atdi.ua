﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.MonitoringProcess.ProcessSignal
{
    [Serializable]
    public class BlockOfSignal
    {
        public float[] IQStream;
        public int StartIndexIQ;
        public Double Durationmks; //длительность блока в микросекундах
    }
}
