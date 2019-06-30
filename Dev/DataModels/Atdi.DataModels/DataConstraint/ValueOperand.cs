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
    [KnownType(typeof(CharsValueOperand))]
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

        public static ValueOperand Create(DataType dataType, object value)
        {
            switch (dataType)
            {
                case DataType.Undefined:
                    return new ValueOperand
                    {
                        DataType = DataType.Undefined
                    };
                case DataType.String:
                    return new StringValueOperand
                    {
                        Value = Convert.ToString(value)
                    };
                case DataType.Boolean:
                    return new BooleanValueOperand
                    {
                        Value = (value == null) ? (bool?)null : Convert.ToBoolean(value)
                    };
                case DataType.Integer:
                    return new IntegerValueOperand
                    {
                        Value = (value == null) ? (int?)null : Convert.ToInt32(value)
                    };
                case DataType.DateTime:
                    return new DateTimeValueOperand
                    {
                        Value = (value == null) ? (DateTime?)null : Convert.ToDateTime(value)
                    };
                case DataType.Double:
                    return new DoubleValueOperand
                    { 
                        Value = (value == null) ? (double?)null : Convert.ToDouble(value)
                    };
                case DataType.Float:
                    return new FloatValueOperand
                    {
                        Value = (value == null) ? (float?)null : (float)Convert.ToDouble(value)
                    };
                case DataType.Decimal:
                    return new DecimalValueOperand
                    {
                        Value = (value == null) ? (decimal?)null : Convert.ToDecimal(value)
                    };
                case DataType.Byte:
                    return new ByteValueOperand
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value)
                    };
                case DataType.Bytes:
                    return new BytesValueOperand
                    {
                        Value = (value == null) ? (byte[])null : (byte[])(value)
                    };
                case DataType.Guid:
                    return new GuidValueOperand
                    {
                        Value = (value == null) ? (Guid?)null : (Guid)(value)
                    };
                case DataType.Char:
                    return new CharValueOperand
                    {
                        Value = (value == null) ? (char?)null : Convert.ToChar(value)
                    };
                case DataType.Chars:
                    return new CharsValueOperand
                    {
                        Value = (value == null) ? (char[])null : (char[])(value)
                    };
                case DataType.Short:
                    return new ShortValueOperand
                    {
                        Value = (value == null) ? (short?)null : Convert.ToInt16(value)
                    };
                case DataType.UnsignedShort:
                    return new UnsignedShortValueOperand
                    {
                        Value = (value == null) ? (ushort?)null : Convert.ToUInt16(value)
                    };
                case DataType.UnsignedInteger:
                    return new UnsignedIntegerValueOperand
                    {
                        Value = (value == null) ? (uint?)null : Convert.ToUInt32(value)
                    };
                case DataType.Long:
                    return new LongValueOperand
                    {
                        Value = (value == null) ? (long?)null : Convert.ToInt64(value)
                    };
                case DataType.UnsignedLong:
                    return new UnsignedLongValueOperand
                    {
                        Value = (value == null) ? (ulong?)null : Convert.ToUInt64(value)
                    };
                case DataType.SignedByte:
                    return new SignedByteValueOperand
                    {
                        Value = (value == null) ? (sbyte?)null : Convert.ToSByte(value)
                    };
                case DataType.Time:
                    return new TimeValueOperand
                    {
                        Value = (value == null) ? (TimeSpan?)null : (TimeSpan)(value)
                    };
                case DataType.Date:
                    return new DateValueOperand
                    {
                        Value = (value == null) ? (DateTime?)null : Convert.ToDateTime(value)
                    };
                case DataType.DateTimeOffset:
                    return new DateTimeOffsetValueOperand
                    {
                        Value = (value == null) ? (DateTimeOffset?)null : (DateTimeOffset)(value)
                    };
                default:
                    throw new InvalidOperationException($"Unsupported the data type with name '{dataType}'");
            }
        }

        public static ValueOperand Create<TValue>(TValue value)
        {
            var type = typeof(TValue);
            if (type == typeof(string))
            {
                return new StringValueOperand
                {
                    Value = (string)(object)value
                };
            }
            if (type == typeof(int) || type == typeof(int?))
            {
                return new IntegerValueOperand
                {
                    Value = (int?)(object)value
                };
            }
            if (type == typeof(Int32) || type == typeof(Int32?))
            {
                return new IntegerValueOperand
                {
                    Value = (Int32?)(object)value
                };
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                return new BooleanValueOperand
                {
                    Value = (bool?)(object)value
                };
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return new DateTimeValueOperand
                {
                    Value = (DateTime?)(object)value
                };
            }
            if (type == typeof(float) || type == typeof(float?))
            {
                return new FloatValueOperand
                {
                    Value = (float?)(object)value
                };
            }
            if (type == typeof(double) || type == typeof(double?))
            {
                return new DoubleValueOperand
                {
                    Value = (double?)(object)value
                };
            }
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return new DecimalValueOperand
                {
                    Value = (decimal?)(object)value
                };
            }
            if (type == typeof(byte) || type == typeof(byte?))
            {
                return new ByteValueOperand
                {
                    Value = (byte?)(object)value
                };
            }
            if (type == typeof(byte[]))
            {
                return new BytesValueOperand
                {
                    Value = (byte[])(object)value
                };
            }
            if (type == typeof(Guid) || type == typeof(Guid?))
            {
                return new GuidValueOperand
                {
                    Value = (Guid?)(object)value
                };
            }
            if (type == typeof(char) || type == typeof(char?))
            {
                return new CharValueOperand
                {
                    Value = (char?)(object)value
                };
            }
            if (type == typeof(char) || type == typeof(char?))
            {
                return new CharValueOperand
                {
                    Value = (char?)(object)value
                };
            }
            if (type == typeof(short) || type == typeof(short?))
            {
                return new ShortValueOperand
                {
                    Value = (short?)(object)value
                };
            }
            if (type == typeof(UInt16) || type == typeof(UInt16?))
            {
                return new UnsignedShortValueOperand
                {
                    Value = (UInt16?)(object)value
                };
            }
            if (type == typeof(UInt32) || type == typeof(UInt32?))
            {
                return new UnsignedIntegerValueOperand
                {
                    Value = (UInt32?)(object)value
                };
            }
            if (type == typeof(long) || type == typeof(long?))
            {
                return new LongValueOperand
                {
                    Value = (long?)(object)value
                };
            }
            if (type == typeof(UInt64) || type == typeof(UInt64?))
            {
                return new UnsignedLongValueOperand
                {
                    Value = (UInt64?)(object)value
                };
            }
            if (type == typeof(sbyte) || type == typeof(sbyte?))
            {
                return new SignedByteValueOperand
                {
                    Value = (sbyte?)(object)value
                };
            }
            if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
            {
                return new TimeValueOperand
                {
                    Value = (TimeSpan?)(object)value
                };
            }
            if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            {
                return new DateTimeOffsetValueOperand
                {
                    Value = (DateTimeOffset?)(object)value
                };
            }

            throw new InvalidOperationException($"Value type not supported '{type}')");
        }

        public object GetValue()
        {
            switch (this.DataType)
            {
                case DataType.String:
                    return ((StringValueOperand)this).Value;
                case DataType.Integer:
                    return ((IntegerValueOperand)this).Value;
                case DataType.Float:
                    return ((FloatValueOperand)this).Value;
                case DataType.Double:
                    return ((DoubleValueOperand)this).Value;
                case DataType.Boolean:
                    return ((BooleanValueOperand)this).Value;
                case DataType.DateTime:
                    return ((DateTimeValueOperand)this).Value;
                case DataType.Decimal:
                    return ((DecimalValueOperand)this).Value;
                case DataType.Byte:
                    return ((ByteValueOperand)this).Value;
                case DataType.Bytes:
                    return ((BytesValueOperand)this).Value;
                case DataType.Guid:
                    return ((GuidValueOperand)this).Value;
                case DataType.Char:
                    return ((CharValueOperand)this).Value;
                case DataType.Chars:
                    return ((CharsValueOperand)this).Value;
                case DataType.Short:
                    return ((ShortValueOperand)this).Value;
                case DataType.UnsignedShort:
                    return ((UnsignedShortValueOperand)this).Value;
                case DataType.UnsignedInteger:
                    return ((UnsignedIntegerValueOperand)this).Value;
                case DataType.Long:
                    return ((LongValueOperand)this).Value;
                case DataType.UnsignedLong:
                    return ((UnsignedLongValueOperand)this).Value;
                case DataType.SignedByte:
                    return ((SignedByteValueOperand)this).Value;
                case DataType.Time:
                    return ((TimeValueOperand)this).Value;
                case DataType.Date:
                    return ((DateValueOperand)this).Value;
                case DataType.DateTimeOffset:
                    return ((DateTimeOffsetValueOperand)this).Value;
                case DataType.Xml:
                    return ((XmlValueOperand)this).Value;
                case DataType.Json:
                    return ((JsonValueOperand)this).Value;
                case DataType.ClrEnum:
                    return ((ClrEnumValueOperand)this).Value;
                case DataType.ClrType:
                    return ((ClrTypeValueOperand)this).Value; ;
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{this.DataType}'");
            }
        }
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
    public class CharsValueOperand : ValueOperand
    {
        public CharsValueOperand()
        {
            this.DataType = DataType.Char;
        }

        [DataMember]
        public char[] Value { get; set; }
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
