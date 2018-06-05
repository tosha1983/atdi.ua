using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.DataConstraint
{
    [KnownType(typeof(StringValueOperand))]
    [KnownType(typeof(BooleanValueOperand))]
    [KnownType(typeof(IntegerValueOperand))]
    [KnownType(typeof(DateTimeValueOperand))]
    [KnownType(typeof(FloatValueOperand))]
    [KnownType(typeof(DoubleValueOperand))]
    [KnownType(typeof(DecimalValueOperand))]
    [KnownType(typeof(ByteValueOperand))]
    [KnownType(typeof(BytesValueOperand))]
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ValueOperand : Operand
    {
        public ValueOperand()
        {
            this.Type = OperandType.Value;
        }

        [DataMember]
        public DataType DataType { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class StringValueOperand : ValueOperand
    {
        public StringValueOperand()
        {
            this.DataType = DataType.String;
        }

        [DataMember]
        public string Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class BooleanValueOperand : ValueOperand
    {
        public BooleanValueOperand()
        {
            this.DataType = DataType.Boolean;
        }

        [DataMember]
        public bool? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DateTimeValueOperand : ValueOperand
    {
        public DateTimeValueOperand()
        {
            this.DataType = DataType.DateTime;
        }

        [DataMember]
        public DateTime? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class IntegerValueOperand : ValueOperand
    {
        public IntegerValueOperand()
        {
            this.DataType = DataType.DateTime;
        }

        [DataMember]
        public int? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DoubleValueOperand : ValueOperand
    {
        public DoubleValueOperand()
        {
            this.DataType = DataType.Double;
        }

        [DataMember]
        public double? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class FloatValueOperand : ValueOperand
    {
        public FloatValueOperand()
        {
            this.DataType = DataType.Double;
        }

        [DataMember]
        public float? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DecimalValueOperand : ValueOperand
    {
        public DecimalValueOperand()
        {
            this.DataType = DataType.Decimal;
        }

        [DataMember]
        public decimal? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ByteValueOperand : ValueOperand
    {
        public ByteValueOperand()
        {
            this.DataType = DataType.Byte;
        }

        [DataMember]
        public byte? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class BytesValueOperand : ValueOperand
    {
        public BytesValueOperand()
        {
            this.DataType = DataType.Bytes;
        }

        [DataMember]
        public byte?[] Value { get; set; }
    }

}
