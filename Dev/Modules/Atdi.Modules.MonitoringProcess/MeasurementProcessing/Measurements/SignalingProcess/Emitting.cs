using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.SDR.Server.MeasurementProcessing.Measurement
{
    public struct Emitting
    {
        public double StartFrequency_MHz;
        public double StopFrequency_MHz;
        public double MaxPower_dBm;
        public double MinPower_dBm;
        public double AvaragePower_dBm;
        public double ReferenceLevel_dBm;
        public DateTime StartEmitting;
        public DateTime StopEmitting;
        public int Count;
    }
}
