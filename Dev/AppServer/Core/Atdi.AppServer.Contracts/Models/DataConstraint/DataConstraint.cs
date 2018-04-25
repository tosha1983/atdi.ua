using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts
{
    /// <summary>
    /// Represents the options of the executing query
    /// </summary>
    [DataContract(Namespace = CommonServicesSpecification.Namespace)]
    [KnownType(typeof(DataConstraintExpression))]
    [KnownType(typeof(DataConstraintGroup))]
    public class DataConstraint
    {
        [DataMember]
        public DataConstraintType Type;
    }

}
