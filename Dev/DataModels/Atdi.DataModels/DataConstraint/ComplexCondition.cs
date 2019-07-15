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
    public class ComplexCondition : Condition
    {
        public ComplexCondition()
        {
            this.Type = ConditionType.Complex;
        }

        [DataMember]
        public LogicalOperator Operator { get; set; }

        [DataMember]
        public Condition[] Conditions { get; set; }

        public override string ToString()
        {
            return $"Type = '{Type}', Operator = '{Operator}', Conditions = {Conditions?.Length}";
        }
    }
}
