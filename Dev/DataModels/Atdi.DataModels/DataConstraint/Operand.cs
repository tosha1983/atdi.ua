using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Atdi.DataModels.DataConstraint
{
    
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
    [KnownType(typeof(GuidValueOperand))]
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
    [KnownType(typeof(GuidValuesOperand))]
    [KnownType(typeof(CharValuesOperand))]
    [KnownType(typeof(ShortValuesOperand))]
    [KnownType(typeof(UnsignedShortValuesOperand))]
    [KnownType(typeof(UnsignedIntegerValuesOperand))]
    [KnownType(typeof(LongValuesOperand))]
    [KnownType(typeof(UnsignedLongValuesOperand))]
    [KnownType(typeof(SignedByteValuesOperand))]
    [KnownType(typeof(TimeValuesOperand))]
    [KnownType(typeof(DateValuesOperand))]
    [KnownType(typeof(DateTimeOffsetValuesOperand))]
    [KnownType(typeof(XmlValuesOperand))]
    [KnownType(typeof(JsonValuesOperand))]
    [KnownType(typeof(ClrEnumValuesOperand))]
    [KnownType(typeof(ClrTypeValuesOperand))]

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class Operand
    {
        [DataMember]
        public OperandType Type { get; set; }
    }
}
