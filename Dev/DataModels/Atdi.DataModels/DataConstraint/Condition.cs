using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.DataConstraint
{
    /// <summary>
    /// Represents the condition of data filtering
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    [KnownType(typeof(ConditionExpression))]
    [KnownType(typeof(ComplexCondition))]
    public class Condition
    {
        [DataMember]
        public ConditionType Type { get; set; }
    }

}
