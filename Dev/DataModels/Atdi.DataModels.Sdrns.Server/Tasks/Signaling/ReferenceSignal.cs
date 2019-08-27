using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.Sdrns.Server
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class ReferenceSignal
    {
        [DataMember]
        public double Frequency_MHz;
        [DataMember]
        public double Bandwidth_kHz;
        [DataMember]
        public double LevelSignal_dBm;
        [DataMember]
        public SignalMask SignalMask;
        [DataMember]
        public int IcsmId;
        [DataMember]
        public string IcsmTable;
    }
}
