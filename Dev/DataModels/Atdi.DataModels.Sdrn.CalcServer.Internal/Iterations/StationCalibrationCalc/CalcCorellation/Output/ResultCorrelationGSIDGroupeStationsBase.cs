﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
    [Serializable]
    public class ResultCorrelationGSIDGroupeStationsBase
    {
        public double Freq_MHz;
        public float Delta_dB;
        public double Corellation_pc;
        public double DeltaCorrelation_pc;
        public double Corellation_factor;
        public float StdDev_dB;
        public float AvErr_dB;
        public string StatusResult;
        public int CountPoints;
        public int UsedPoints_pc;
        public ContextStation ClientContextStation;
        public DriveTestsResult DriveTestsResult;
        public CalibrationStatusParameters CalibrationStatusParameters;
        public CorrellationPoint[] CorrellationPoints;
    }
}