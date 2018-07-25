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
    public class Filter
    {
        [DataMember]
        public ConditionType Type { get; set; }

        [DataMember]
        public FilterOperand LeftOperand { get; set; }

        [DataMember]
        public ConditionOperator Operator { get; set; }

        [DataMember]
        public FilterOperand RightOperand { get; set; }


        [DataMember]
        public LogicalOperator FilterOperator { get; set; }

        [DataMember]
        public Filter[] Filters { get; set; }

        public static explicit operator Condition(Filter filter)
        {
            if (filter == null)
            {
                return null;
            }

            if (filter.Type == ConditionType.Expression)
            {
                var condition = new ConditionExpression
                {
                    Type = ConditionType.Expression,
                    Operator = filter.Operator,
                    LeftOperand = (Operand)filter.LeftOperand,
                    RightOperand = (Operand)filter.RightOperand
                };

                return condition;
            }

            if (filter.Type == ConditionType.Complex)
            {
                var condition = new ComplexCondition
                {
                    Type = ConditionType.Complex,
                    Operator = filter.FilterOperator,
                    Conditions = filter.Filters.Select(f => (Condition)f).ToArray()
                };

                return condition;
            }

            throw new InvalidOperationException($"Unsupported the filter/condition type with name '{filter.Type}'");
        }
    }
}
