﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
    public class ThresholdFieldStrengthParameters
    {
        public string Standard; // стандарт источника интерференции
        public double MinFreq_MHz;
        public double MaxFreq_MHz;
        public string StaClass;
        public string System_type;
        public bool  IsDigital;
        public float ThresholdFS;
        public float Time_pc;
        public float Height_m;
        public bool IsBaseStation;
    }
}
    
