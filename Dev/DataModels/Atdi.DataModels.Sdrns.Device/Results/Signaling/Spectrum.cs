using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    public class Spectrum
    {
        public float[] Levels_dBm;
        public double SpectrumStartFreq_MHz;
        public double SpectrumSteps_kHz;
        public int T1;
        public int T2;
        public int MarkerIndex;
        public double Bandwidth_kHz;
        public bool СorrectnessEstimations; 
        public int TraceCount;
        public float SignalLevel_dBm;
        public bool Contravention; // при нарушении true
    }
}
