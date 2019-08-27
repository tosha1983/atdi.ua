using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server;
using System.Runtime.Serialization;


namespace Atdi.DataModels.Sdrns.Server
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SignalMask
    {
        [DataMember]
        public float[] Loss_dB;
        [DataMember]
        public double[] Freq_kHz;
    }
}
