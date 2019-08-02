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
    public class EmittingParameters
    {
        [DataMember]
        public double RollOffFactor { get; set; } // from 0.85 to 1.35
        [DataMember]
        public double StandardBW { get; set; } // or channel BW
        [DataMember]
        public string Standard { get; set; } // or channel GSM/LTE/UMTS
    }
}
