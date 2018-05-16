using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.WebQuery
{
    /// <summary>
    /// Represents the parameter the web query
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class QueryParameter
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }
    }
}
