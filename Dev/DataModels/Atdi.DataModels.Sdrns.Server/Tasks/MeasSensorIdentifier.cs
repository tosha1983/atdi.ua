using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.DataModels.Sdrns.Server
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class MeasSensorIdentifier
    {
        [DataMember]
        public long Value;
    }
}
