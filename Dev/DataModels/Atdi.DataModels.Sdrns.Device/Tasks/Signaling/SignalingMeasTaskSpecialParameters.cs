using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.Sdrns.Device
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SignalingMeasTaskSpecialParameters
    {
        [DataMember]
        public int? NumberPointForChangeExcess { get; set; }
        [DataMember]
        public double? AllowableExcess_dB { get; set; }
        [DataMember]
        public double? DiffLevelForCalcBW { get; set; }
        [DataMember]
        public double? WindowBW { get; set; }
        [DataMember]
        public double? DbLevel_dB { get; set; }
        [DataMember]
        public int? NumberIgnoredPoints { get; set; }
        [DataMember]
        public double? MinExcessNoseLevel_dB { get; set; }
        [DataMember]
        public double? NoiseLevel_dBm_Hz { get; set; }
        [DataMember]
        public int? TimeBetweenWorkTimes_sec { get; set; }
        [DataMember]
        public int? TypeJoinSpectrum { get; set; }
        [DataMember]
        public double? CrossingBWPercentageForGoodSignals { get; set; }
        [DataMember]
        public double? CrossingBWPercentageForBadSignals { get; set; }
        [DataMember]
        public bool? AnalyzeByChannel { get; set; }
        [DataMember]
        public bool? AnalyzeSysInfoEmission { get; set; }
        [DataMember]
        public bool? DetailedMeasurementsBWEmission { get; set; }
        //[DataMember]
        //public string Standard { get; set; }


    }
}
