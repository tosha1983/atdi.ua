using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Represents frequency for measurement 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class GetShortMeasResultsByDateValue
    {
        [DataMember]
        public DateTime? Start;
        [DataMember]
        public DateTime? End;
    }
}

