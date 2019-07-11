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
    public class SignalMask
    {
        [DataMember]
        public float[] Loss_dB { get; set; }
        [DataMember]
        public double[] Freq_kHz { get; set; }
    }
}
