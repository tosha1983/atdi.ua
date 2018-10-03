using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class MeasurementResultIdentifier
    {
        [DataMember]
        public int Value;
    }
}
