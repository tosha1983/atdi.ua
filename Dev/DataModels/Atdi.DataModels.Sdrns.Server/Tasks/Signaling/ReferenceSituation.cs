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
    public class ReferenceSituation
    {
        [DataMember]
        public ReferenceSignal[] ReferenceSignal;
        [DataMember]
        public long SensorId;
    }
}
