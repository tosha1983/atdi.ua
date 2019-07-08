using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.DataConstraint
{
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class FilterOperand
    {
        [DataMember]
        public OperandType Type { get; set; }

        [DataMember]
        public string ColumnSource { get; set; }

        [DataMember]
        public string ColumnName { get; set; }

        [DataMember]
        public DataType DataType { get; set; }

        [DataMember]
        public object Value { get; set; }

        [DataMember]
        public object[] Values { get; set; }

        public static T ToEnum<T>(string enumString)
        {
            return (T)Enum.Parse(typeof(T), enumString);
        }

        public static explicit operator Operand(FilterOperand filterOperand)
        {
            if (filterOperand == null)
            {
                return null;
            }

            if (filterOperand.Type == OperandType.Column)
            {
                var operand = new ColumnOperand
                {
                    Type = OperandType.Column,
                    Source = filterOperand.ColumnSource,
                    ColumnName = filterOperand.ColumnName
                };

                return operand;
            }

            if (filterOperand.Type == OperandType.Value)
            {
                return ValueOperand.Create(filterOperand.DataType, filterOperand.Value);
            }

            if (filterOperand.Type == OperandType.Values)
            {
                return ValuesOperand.Create(filterOperand.DataType, filterOperand.Values);
            }

            throw new InvalidOperationException($"Unsupported the filter operand type with name '{filterOperand.Type}'");
        }
    }
}
