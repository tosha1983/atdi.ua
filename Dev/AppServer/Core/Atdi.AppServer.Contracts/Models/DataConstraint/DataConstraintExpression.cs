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
    public class DataConstraintExpression : DataConstraint
    {
        public DataConstraintExpression()
        {
            this.Type = DataConstraintType.Expression;
        }

        [DataMember]
        public DataConstraintOperand LeftOperand;
        [DataMember]
        public DataConstraintOperation Operation;
        [DataMember]
        public DataConstraintOperand RightOperand;
    }
}
