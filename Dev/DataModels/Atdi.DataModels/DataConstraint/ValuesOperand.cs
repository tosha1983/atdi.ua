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
    [KnownType(typeof(CharsValuesOperand))]
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

        public static ValuesOperand Create(DataType dataType, object[] values)
        {
            switch (dataType)
            {
                case DataType.Undefined:
                    return new ValuesOperand
                    {
                        DataType = DataType.Undefined
                    };
                case DataType.String:
                    return new StringValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? null : Convert.ToString(v)).ToArray()
                    };
                case DataType.Boolean:
                    return new BooleanValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (bool?)null : Convert.ToBoolean(v)).ToArray()
                    };
                case DataType.Integer:
                    return new IntegerValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (int?)null : Convert.ToInt32(v)).ToArray()
                    };
                case DataType.DateTime:
                    return new DateTimeValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (DateTime?)null : Convert.ToDateTime(v)).ToArray()
                    };
                case DataType.Double:
                    return new DoubleValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (double?)null : Convert.ToDouble(v)).ToArray()
                    };
                case DataType.Float:
                    return new FloatValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (float?)null : (float)Convert.ToDouble(v)).ToArray()
                    };
                case DataType.Decimal:
                    return new DecimalValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (decimal?)null : Convert.ToDecimal(v)).ToArray()
                    };
                case DataType.Byte:
                    return new ByteValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (byte?)null : Convert.ToByte(v)).ToArray()
                    };
                case DataType.Bytes:
                    return new BytesValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (byte[])null : (byte[])(v)).ToArray()
                    };
                case DataType.Guid:
                    return new GuidValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (Guid?)null : (Guid)(v)).ToArray()
                    };
                case DataType.Char:
                    return new CharValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (char?)null : Convert.ToChar(v)).ToArray()
                    };
                case DataType.Chars:
                    return new CharsValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (char[])null : (char[])(v)).ToArray()
                    };
                case DataType.Short:
                    return new ShortValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (short?)null : Convert.ToInt16(v)).ToArray()
                    };
                case DataType.UnsignedShort:
                    return new UnsignedShortValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (ushort?)null : Convert.ToUInt16(v)).ToArray()
                    };
                case DataType.UnsignedInteger:
                    return new UnsignedIntegerValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (uint?)null : Convert.ToUInt32(v)).ToArray()
                    };
                case DataType.Long:
                    return new LongValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (long?)null : Convert.ToInt64(v)).ToArray()
                    };
                case DataType.UnsignedLong:
                    return new UnsignedLongValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (ulong?)null : Convert.ToUInt64(v)).ToArray()
                    };
                case DataType.SignedByte:
                    return new SignedByteValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (sbyte?)null : Convert.ToSByte(v)).ToArray()
                    };
                case DataType.Time:
                    return new TimeValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (TimeSpan?)null : (TimeSpan)(v)).ToArray()
                    };
                case DataType.Date:
                    return new DateValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (DateTime?)null : Convert.ToDateTime(v)).ToArray()
                    };
                case DataType.DateTimeOffset:
                    return new DateTimeOffsetValuesOperand
                    {
                        Values = values?.Select(v => (v == null) ? (DateTimeOffset?)null : (DateTimeOffset)(v)).ToArray()
                    };
               
                default:
                    throw new InvalidOperationException($"Unsupported the data type with name '{dataType}'");
            }
        }

        public static ValuesOperand Create<TValue>(params TValue[] values)
        {
            var type = typeof(TValue);
            if (type == typeof(string))
            {
                return new StringValuesOperand
                {
                    Values = values.Select(o => (string)(object)o).ToArray()
                };
            }
            if (type == typeof(int) || type == typeof(int?))
            {
                return new IntegerValuesOperand
                {
                    Values = values.Select(o => (int?)(object)o).ToArray()
                };
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                return new BooleanValuesOperand
                {
                    Values = values.Select(o => (bool?)(object)o).ToArray()
                };
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return new DateTimeValuesOperand
                {
                    Values = values.Select(o => (DateTime?)(object)o).ToArray()
                };
            }
            if (type == typeof(float) || type == typeof(float?))
            {
                return new FloatValuesOperand
                {
                    Values = values.Select(o => (float?)(object)o).ToArray()
                };
            }
            if (type == typeof(double) || type == typeof(double?))
            {
                return new DoubleValuesOperand
                {
                    Values = values.Select(o => (double?)(object)o).ToArray()
                };
            }
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return new DecimalValuesOperand
                {
                    Values = values.Select(o => (decimal?)(object)o).ToArray()
                };
            }
            if (type == typeof(byte) || type == typeof(byte?))
            {
                return new ByteValuesOperand
                {
                    Values = values.Select(o => (byte?)(object)o).ToArray()
                };
            }
            if (type == typeof(byte[]))
            {
                return new BytesValuesOperand
                {
                    Values = values.Select(o => (byte[])(object)o).ToArray()
                };
            }
            if (type == typeof(Guid) || type == typeof(Guid?))
            {
                return new GuidValuesOperand
                {
                    Values = values.Select(o => (Guid?)(object)o).ToArray()
                };
            }
            if (type == typeof(char) || type == typeof(char?))
            {
                return new CharValuesOperand
                {
                    Values = values.Select(o => (char?)(object)o).ToArray()
                };
            }
            if (type == typeof(short) || type == typeof(short?))
            {
                return new ShortValuesOperand
                {
                    Values = values.Select(o => (short?)(object)o).ToArray()
                };
            }
            if (type == typeof(UInt16) || type == typeof(UInt16?))
            {
                return new UnsignedShortValuesOperand
                {
                    Values = values.Select(o => (UInt16?)(object)o).ToArray()
                };
            }
            if (type == typeof(UInt32) || type == typeof(UInt32?))
            {
                return new UnsignedIntegerValuesOperand
                {
                    Values = values.Select(o => (UInt32?)(object)o).ToArray()
                };
            }
            if (type == typeof(long) || type == typeof(long?))
            {
                return new LongValuesOperand
                {
                    Values = values.Select(o => (long?)(object)o).ToArray()
                };
            }
            if (type == typeof(UInt64) || type == typeof(UInt64?))
            {
                return new UnsignedLongValuesOperand
                {
                    Values = values.Select(o => (UInt64?)(object)o).ToArray()
                };
            }
            if (type == typeof(sbyte) || type == typeof(sbyte?))
            {
                return new SignedByteValuesOperand
                {
                    Values = values.Select(o => (sbyte?)(object)o).ToArray()
                };
            }
            if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
            {
                return new TimeValuesOperand
                {
                    Values = values.Select(o => (TimeSpan?)(object)o).ToArray()
                };
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return new DateValuesOperand
                {
                    Values = values.Select(o => (DateTime?)(object)o).ToArray()
                };
            }
            if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            {
                return new DateTimeOffsetValuesOperand
                {
                    Values = values.Select(o => (DateTimeOffset?)(object)o).ToArray()
                };
            }
            if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            {
                return new DateTimeOffsetValuesOperand
                {
                    Values = values.Select(o => (DateTimeOffset?)(object)o).ToArray()
                };
            }
            if (type == typeof(Enum))
            {
                return new ClrEnumValuesOperand
                {
                    Values = values.Select(o => (Enum)(object)o).ToArray()
                };
            }
            if (type == typeof(Object))
            {
                return new ClrTypeValuesOperand
                {
                    Values = values.Select(o => (Object)(object)o).ToArray()
                };
            }

            throw new InvalidOperationException($"Value type not supported '{type}')");
        }
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
    public class CharsValuesOperand : ValuesOperand
    {
        public CharsValuesOperand()
        {
            this.DataType = DataType.Chars;
        }

        [DataMember]
        public char[][] Values { get; set; }
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
