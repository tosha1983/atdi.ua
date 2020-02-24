using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.Modules.Sdrn.Calculation
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class Spectrum
    {
        [DataMember]
        public float[] Levels_dBm { get; set; }
        [DataMember]
        public double SpectrumStartFreq_MHz { get; set; }
        [DataMember]
        public double SpectrumSteps_kHz { get; set; }
        [DataMember]
        public int T1 { get; set; }
        [DataMember]
        public int T2 { get; set; }
        [DataMember]
        public int MarkerIndex { get; set; }
        [DataMember]
        public double Bandwidth_kHz { get; set; }
        [DataMember]
        public bool СorrectnessEstimations { get; set; }
        [DataMember]
        public int TraceCount { get; set; }
        [DataMember]
        public float SignalLevel_dBm { get; set; }
        [DataMember]
        public bool Contravention { get; set; } // при нарушении true
    }
}
