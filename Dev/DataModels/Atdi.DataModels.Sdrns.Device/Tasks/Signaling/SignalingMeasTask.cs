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
    public class SignalingMeasTask
    {
        [DataMember]
        public bool? CompareTraceJustWithRefLevels { get; set; }
        [DataMember]
        public bool? AutoDivisionEmitting { get; set; }
        [DataMember]
        public double? DifferenceMaxMax { get; set; }
        [DataMember]
        public bool? FiltrationTrace { get; set; }
        [DataMember]
        public double? allowableExcess_dB { get; set; }
        [DataMember]
        public int? SignalizationNCount { get; set; }
        [DataMember]
        public int? SignalizationNChenal { get; set; }
        [DataMember]
        public SignalingMeasTaskSpecialParameters SpecialParameters { get; set; }
    }
}
