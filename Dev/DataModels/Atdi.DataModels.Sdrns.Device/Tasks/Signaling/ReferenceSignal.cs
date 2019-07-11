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
    public class ReferenceSignal
    {
        [DataMember]
        public double Frequency_MHz { get; set; }
        [DataMember]
        public double Bandwidth_kHz { get; set; }
        [DataMember]
        public double LevelSignal_dBm { get; set; }
        [DataMember]
        public SignalMask SignalMask { get; set; }
        [DataMember]
        public int IcsmId { get; set; }
        [DataMember]
        public string IcsmTable { get; set; }
    }
}
