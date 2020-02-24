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
    public class WorkTime
    {
        [DataMember]
        public DateTime StartEmitting { get; set; }
        [DataMember]
        public DateTime StopEmitting { get; set; }
        [DataMember]
        public int HitCount { get; set; }
        [DataMember]
        public float PersentAvailability { get; set; }
        [DataMember]
        public int ScanCount { get; set; }
        [DataMember]
        public int TempCount { get; set; }
    }
}
