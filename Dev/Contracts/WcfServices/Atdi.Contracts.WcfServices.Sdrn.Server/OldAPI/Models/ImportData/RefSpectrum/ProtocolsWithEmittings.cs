using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
    public class ProtocolsWithEmittings  
    {
        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        public double? Probability { get; set; }

        [DataMember]
        public double? StartFrequency_MHz { get; set; }

        [DataMember]
        public double? StopFrequency_MHz { get; set; }

        [DataMember]
        public double? CurentPower_dBm { get; set; }

        [DataMember]
        public double? ReferenceLevel_dBm { get; set; }

        [DataMember]
        public double? MeanDeviationFromReference { get; set; } // отклонение формы от эталонной в долях от 0 до 1

        [DataMember]
        public double? TriggerDeviationFromReference { get; set; } // максимально допустимое отклонение формы от эталонной в долях от 0 до 1

        [DataMember]
        public DateTime? WorkTimeStart { get; set; }

        [DataMember]
        public DateTime? WorkTimeStop { get; set; }

        [DataMember]
        public float[] Loss_dB { get; set; }

        [DataMember]
        public double[] Freq_kHz { get; set; }

        [DataMember]
        public float[] Levels_dBm { get; set; }

        [DataMember]
        public double? SpectrumStartFreq_MHz { get; set; }

        [DataMember]
        public double? SpectrumSteps_kHz { get; set; }

        [DataMember]
        public int? T1 { get; set; }

        [DataMember]
        public int? T2 { get; set; }

        [DataMember]
        public int? MarkerIndex { get; set; }

        [DataMember]
        public double? Bandwidth_kHz { get; set; }

        [DataMember]
        public bool CorrectnessEstimations { get; set; }

        [DataMember]
        public int? TraceCount { get; set; }

        [DataMember]
        public float? SignalLevel_dBm { get; set; }

        [DataMember]
        public bool Contravention { get; set; } // при нарушении true

        [DataMember]
        public int[] Levels { get; set; }

        [DataMember]
        public int[] Count { get; set; }

        [DataMember]
        public double? RollOffFactor { get; set; } // from 0.85 to 1.35

        [DataMember]
        public double? StandardBW { get; set; } // or channel BW
        
    }
}
