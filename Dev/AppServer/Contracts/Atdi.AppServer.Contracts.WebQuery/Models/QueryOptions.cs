using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// Represents the options of the executing query
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class QueryOptions
    {
        [DataMember]
        public QueryReference QueryRef;
        [DataMember]
        public DataConstraint Constraint;
    }
}
