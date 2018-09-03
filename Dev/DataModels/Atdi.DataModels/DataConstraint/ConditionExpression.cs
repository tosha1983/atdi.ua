using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.DataConstraint
{
    /// <summary>
    /// Represents the options of the executing query
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ConditionExpression : Condition
    {
        public ConditionExpression()
        {
            this.Type = ConditionType.Expression;
        }

        [DataMember]
        public Operand LeftOperand { get; set; }

        [DataMember]
        public ConditionOperator Operator { get; set; }

        [DataMember]
        public Operand RightOperand { get; set; }

    }
}
