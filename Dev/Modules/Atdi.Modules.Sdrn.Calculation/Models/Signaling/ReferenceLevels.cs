using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;



namespace Atdi.Modules.Sdrn.Calculation
{
    /// <summary>
    /// Represent triggers levels adopted to trace of Devise (SDR)
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class ReferenceLevels
    {
        [DataMember]
        public double StartFrequency_Hz { get; set; }
        [DataMember]
        public double StepFrequency_Hz { get; set; }
        [DataMember]
        public float[] levels { get; set; }
    }
}
