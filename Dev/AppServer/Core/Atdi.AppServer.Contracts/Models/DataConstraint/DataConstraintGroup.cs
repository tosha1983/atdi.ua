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
    public class DataConstraintGroup : DataConstraint
    {
        public DataConstraintGroup()
        {
            this.Type = DataConstraintType.Group;
        }

        [DataMember]
        public DataConstraintGroupOperation Operation;
        [DataMember]
        public DataConstraint[] Constraints;
    }
}
