using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Atdi.DataModels.DataConstraint
{
    [DataContract(Namespace = CommonSpecification.Namespace)]
    [KnownType(typeof(ValueOperand))]
    [KnownType(typeof(StringValueOperand))]
    [KnownType(typeof(BooleanValueOperand))]
    [KnownType(typeof(IntegerValueOperand))]
    [KnownType(typeof(DateTimeValueOperand))]
    [KnownType(typeof(FloatValueOperand))]
    [KnownType(typeof(DoubleValueOperand))]
    [KnownType(typeof(DecimalValueOperand))]
    [KnownType(typeof(ByteValueOperand))]
    [KnownType(typeof(BytesValueOperand))]
    [KnownType(typeof(ColumnOperand))]
    [KnownType(typeof(ValuesOperand))]
    [KnownType(typeof(StringValuesOperand))]
    [KnownType(typeof(BooleanValuesOperand))]
    [KnownType(typeof(IntegerValuesOperand))]
    [KnownType(typeof(DateTimeValuesOperand))]
    [KnownType(typeof(FloatValuesOperand))]
    [KnownType(typeof(DoubleValuesOperand))]
    [KnownType(typeof(DecimalValuesOperand))]
    [KnownType(typeof(ByteValuesOperand))]
    [KnownType(typeof(BytesValuesOperand))]
    public class Operand
    {
        [DataMember]
        public OperandType Type { get; set; }
    }
}
