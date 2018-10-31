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
    [KnownType(typeof(GuidValueOperand))]
    [KnownType(typeof(CharValueOperand))]
    [KnownType(typeof(ShortValueOperand))]
    [KnownType(typeof(UnsignedShortValueOperand))]
    [KnownType(typeof(UnsignedIntegerValueOperand))]
    [KnownType(typeof(LongValueOperand))]
    [KnownType(typeof(UnsignedLongValueOperand))]
    [KnownType(typeof(SignedByteValueOperand))]
    [KnownType(typeof(TimeValueOperand))]
    [KnownType(typeof(DateValueOperand))]
    [KnownType(typeof(DateTimeOffsetValueOperand))]
    [KnownType(typeof(XmlValueOperand))]
    [KnownType(typeof(JsonValueOperand))]
    [KnownType(typeof(ClrEnumValueOperand))]
    [KnownType(typeof(ClrTypeValueOperand))]

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
            this.DataType = DataType.Integer;
        }

        [DataMember]
        public Int32? Value { get; set; }
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
            this.DataType = DataType.Float;
        }

        [DataMember]
        public Single? Value { get; set; }
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
        public byte[] Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class GuidValueOperand : ValueOperand
    {
        public GuidValueOperand()
        {
            this.DataType = DataType.Guid;
        }

        [DataMember]
        public Guid? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class CharValueOperand : ValueOperand
    {
        public CharValueOperand()
        {
            this.DataType = DataType.Char;
        }

        [DataMember]
        public char? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ShortValueOperand : ValueOperand
    {
        public ShortValueOperand()
        {
            this.DataType = DataType.Short;
        }

        [DataMember]
        public Int16? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class UnsignedShortValueOperand : ValueOperand
    {
        public UnsignedShortValueOperand()
        {
            this.DataType = DataType.UnsignedShort;
        }

        [DataMember]
        public UInt16? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class UnsignedIntegerValueOperand : ValueOperand
    {
        public UnsignedIntegerValueOperand()
        {
            this.DataType = DataType.UnsignedInteger;
        }

        [DataMember]
        public UInt32? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class LongValueOperand : ValueOperand
    {
        public LongValueOperand()
        {
            this.DataType = DataType.Long;
        }

        [DataMember]
        public Int64? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class UnsignedLongValueOperand : ValueOperand
    {
        public UnsignedLongValueOperand()
        {
            this.DataType = DataType.UnsignedLong;
        }

        [DataMember]
        public UInt64? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class SignedByteValueOperand : ValueOperand
    {
        public SignedByteValueOperand()
        {
            this.DataType = DataType.SignedByte;
        }

        [DataMember]
        public sbyte? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class TimeValueOperand : ValueOperand
    {
        public TimeValueOperand()
        {
            this.DataType = DataType.Time;
        }

        [DataMember]
        public TimeSpan? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DateValueOperand : ValueOperand
    {
        public DateValueOperand()
        {
            this.DataType = DataType.Date;
        }

        [DataMember]
        public DateTime? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DateTimeOffsetValueOperand : ValueOperand
    {
        public DateTimeOffsetValueOperand()
        {
            this.DataType = DataType.DateTimeOffset;
        }

        [DataMember]
        public DateTimeOffset? Value { get; set; }
    }


    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class XmlValueOperand : ValueOperand
    {
        public XmlValueOperand()
        {
            this.DataType = DataType.String;
        }

        [DataMember]
        public string Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class JsonValueOperand : ValueOperand
    {
        public JsonValueOperand()
        {
            this.DataType = DataType.String;
        }

        [DataMember]
        public string Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ClrEnumValueOperand : ValueOperand
    {
        public ClrEnumValueOperand()
        {
            this.DataType = DataType.ClrEnum;
        }

        [DataMember]
        public Enum Value { get; set; }
    }


    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ClrTypeValueOperand : ValueOperand
    {
        public ClrTypeValueOperand()
        {
            this.DataType = DataType.ClrType;
        }

        [DataMember]
        public Object Value { get; set; }
    }

}
