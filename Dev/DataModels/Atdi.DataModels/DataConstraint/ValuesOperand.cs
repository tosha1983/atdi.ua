using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.DataConstraint
{
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
    public class ValuesOperand : Operand
    {
        public ValuesOperand()
        {
            this.Type = OperandType.Values;
        }

        [DataMember]
        public DataType DataType { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class StringValuesOperand : ValuesOperand
    {
        public StringValuesOperand()
        {
            this.DataType = DataType.String;
        }

        [DataMember]
        public string[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class BooleanValuesOperand : ValuesOperand
    {
        public BooleanValuesOperand()
        {
            this.DataType = DataType.Boolean;
        }

        [DataMember]
        public bool?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DateTimeValuesOperand : ValuesOperand
    {
        public DateTimeValuesOperand()
        {
            this.DataType = DataType.DateTime;
        }

        [DataMember]
        public DateTime?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class IntegerValuesOperand : ValuesOperand
    {
        public IntegerValuesOperand()
        {
            this.DataType = DataType.Integer;
        }

        [DataMember]
        public Int32?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DoubleValuesOperand : ValuesOperand
    {
        public DoubleValuesOperand()
        {
            this.DataType = DataType.Double;
        }

        [DataMember]
        public double?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class FloatValuesOperand : ValuesOperand
    {
        public FloatValuesOperand()
        {
            this.DataType = DataType.Float;
        }

        [DataMember]
        public Single?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DecimalValuesOperand : ValuesOperand
    {
        public DecimalValuesOperand()
        {
            this.DataType = DataType.Decimal;
        }

        [DataMember]
        public decimal?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ByteValuesOperand : ValuesOperand
    {
        public ByteValuesOperand()
        {
            this.DataType = DataType.Byte;
        }

        [DataMember]
        public byte?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class BytesValuesOperand : ValuesOperand
    {
        public BytesValuesOperand()
        {
            this.DataType = DataType.Bytes;
        }

        [DataMember]
        public byte[][] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class GuidValuesOperand : ValuesOperand
    {
        public GuidValuesOperand()
        {
            this.DataType = DataType.Guid;
        }

        [DataMember]
        public Guid?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class CharValuesOperand : ValuesOperand
    {
        public CharValuesOperand()
        {
            this.DataType = DataType.Char;
        }

        [DataMember]
        public char?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ShortValuesOperand : ValuesOperand
    {
        public ShortValuesOperand()
        {
            this.DataType = DataType.Short;
        }

        [DataMember]
        public Int16?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class UnsignedShortValuesOperand : ValuesOperand
    {
        public UnsignedShortValuesOperand()
        {
            this.DataType = DataType.UnsignedShort;
        }

        [DataMember]
        public UInt16?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class UnsignedIntegerValuesOperand : ValuesOperand
    {
        public UnsignedIntegerValuesOperand()
        {
            this.DataType = DataType.UnsignedInteger;
        }

        [DataMember]
        public UInt32?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class LongValuesOperand : ValuesOperand
    {
        public LongValuesOperand()
        {
            this.DataType = DataType.Long;
        }

        [DataMember]
        public Int64?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class UnsignedLongValuesOperand : ValuesOperand
    {
        public UnsignedLongValuesOperand()
        {
            this.DataType = DataType.UnsignedLong;
        }

        [DataMember]
        public UInt64?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class SignedByteValuesOperand : ValuesOperand
    {
        public SignedByteValuesOperand()
        {
            this.DataType = DataType.SignedByte;
        }

        [DataMember]
        public sbyte?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class TimeValuesOperand : ValuesOperand
    {
        public TimeValuesOperand()
        {
            this.DataType = DataType.Time;
        }

        [DataMember]
        public TimeSpan?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DateValuesOperand : ValuesOperand
    {
        public DateValuesOperand()
        {
            this.DataType = DataType.Date;
        }

        [DataMember]
        public DateTime?[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DateTimeOffsetValuesOperand : ValuesOperand
    {
        public DateTimeOffsetValuesOperand()
        {
            this.DataType = DataType.DateTimeOffset;
        }

        [DataMember]
        public DateTimeOffset?[] Values { get; set; }
    }


    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class XmlValuesOperand : ValuesOperand
    {
        public XmlValuesOperand()
        {
            this.DataType = DataType.String;
        }

        [DataMember]
        public string[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class JsonValuesOperand : ValuesOperand
    {
        public JsonValuesOperand()
        {
            this.DataType = DataType.String;
        }

        [DataMember]
        public string[] Values { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ClrEnumValuesOperand : ValuesOperand
    {
        public ClrEnumValuesOperand()
        {
            this.DataType = DataType.ClrEnum;
        }

        [DataMember]
        public Enum[] Values { get; set; }
    }


    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ClrTypeValuesOperand : ValuesOperand
    {
        public ClrTypeValuesOperand()
        {
            this.DataType = DataType.ClrType;
        }

        [DataMember]
        public Object[] Values { get; set; }
    }

}
