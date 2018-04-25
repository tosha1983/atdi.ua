using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// Represents the parameter the web query
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class QueryParameter
    {
        [DataMember]
        public string Name;
        [DataMember]
        public string Value;
    }
}
