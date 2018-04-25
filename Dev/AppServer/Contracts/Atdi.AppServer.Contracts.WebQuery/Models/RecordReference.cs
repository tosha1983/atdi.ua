using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// Represents the reference to record
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class RecordReference
    {
        [DataMember]
        public int Id;
    }
}
