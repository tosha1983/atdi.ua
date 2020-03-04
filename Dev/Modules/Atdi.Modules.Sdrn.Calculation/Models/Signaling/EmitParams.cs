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
    public class EmitParams
    {
        [DataMember]
        public long EmittingId { get; set; }
        [DataMember]
        public double? CrossingBWPercentageForGoodSignals { get; set; }
        [DataMember]
        public double? CrossingBWPercentageForBadSignals { get; set; }
        [DataMember]
        public int? TimeBetweenWorkTimes_sec { get; set; }
        [DataMember]
        public int? TypeJoinSpectrum { get; set; }
        [DataMember]
        public bool? AnalyzeByChannel { get; set; }
        [DataMember]
        public bool? CorrelationAnalize { get; set; }
        [DataMember]
        public double? CorrelationFactor { get; set; }
        [DataMember]
        public double? MaxFreqDeviation { get; set; }
        [DataMember]
        public bool? CorrelationAdaptation { get; set; }
        [DataMember]
        public int? MaxNumberEmitingOnFreq { get; set; }
        [DataMember]
        public double? MinCoeffCorrelation { get; set; }
        [DataMember]
        public bool? UkraineNationalMonitoring { get; set; }

    }
}
