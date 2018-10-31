using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels
{
    [KnownType(typeof(StringColumnValue))]
    [KnownType(typeof(BooleanColumnValue))]
    [KnownType(typeof(IntegerColumnValue))]
    [KnownType(typeof(DateTimeColumnValue))]
    [KnownType(typeof(FloatColumnValue))]
    [KnownType(typeof(DoubleColumnValue))]
    [KnownType(typeof(DecimalColumnValue))]
    [KnownType(typeof(ByteColumnValue))]
    [KnownType(typeof(BytesColumnValue))]
    [KnownType(typeof(GuidColumnValue))]
    [KnownType(typeof(CharColumnValue))]
    [KnownType(typeof(ShortColumnValue))]
    [KnownType(typeof(UnsignedShortColumnValue))]
    [KnownType(typeof(UnsignedIntegerColumnValue))]
    [KnownType(typeof(LongColumnValue))]
    [KnownType(typeof(SignedByteColumnValue))]
    [KnownType(typeof(UnsignedLongColumnValue))]
    [KnownType(typeof(TimeColumnValue))]
    [KnownType(typeof(DateColumnValue))]
    [KnownType(typeof(DateTimeOffsetColumnValue))]
    [KnownType(typeof(XmlColumnValue))]
    [KnownType(typeof(JsonColumnValue))]
    [KnownType(typeof(ClrEnumColumnValue))]
    [KnownType(typeof(ClrTypeColumnValue))]


    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ColumnValue 
    {
        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DataType DataType { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class StringColumnValue : ColumnValue
    {
        public StringColumnValue()
        {
            this.DataType = DataType.String;
        }

        [DataMember]
        public string Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class BooleanColumnValue : ColumnValue
    {
        public BooleanColumnValue()
        {
            this.DataType = DataType.Boolean;
        }

        [DataMember]
        public bool? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DateTimeColumnValue : ColumnValue
    {
        public DateTimeColumnValue()
        {
            this.DataType = DataType.DateTime;
        }

        [DataMember]
        public DateTime? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class IntegerColumnValue : ColumnValue
    {
        public IntegerColumnValue()
        {
            this.DataType = DataType.Integer;
        }

        [DataMember]
        public Int32? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DoubleColumnValue : ColumnValue
    {
        public DoubleColumnValue()
        {
            this.DataType = DataType.Double;
        }

        [DataMember]
        public double? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class FloatColumnValue : ColumnValue
    {
        public FloatColumnValue()
        {
            this.DataType = DataType.Float;
        }

        [DataMember]
        public Single? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DecimalColumnValue : ColumnValue
    {
        public DecimalColumnValue()
        {
            this.DataType = DataType.Decimal;
        }

        [DataMember]
        public decimal? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ByteColumnValue : ColumnValue
    {
        public ByteColumnValue()
        {
            this.DataType = DataType.Byte;
        }

        [DataMember]
        public byte? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class BytesColumnValue : ColumnValue
    {
        public BytesColumnValue()
        {
            this.DataType = DataType.Bytes;
        }

        [DataMember]
        public byte[] Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class GuidColumnValue : ColumnValue
    {
        public GuidColumnValue()
        {
            this.DataType = DataType.Guid;
        }

        [DataMember]
        public Guid? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class CharColumnValue : ColumnValue
    {
        public CharColumnValue()
        {
            this.DataType = DataType.Char;
        }

        [DataMember]
        public Char? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ShortColumnValue : ColumnValue
    {
        public ShortColumnValue()
        {
            this.DataType = DataType.Short;
        }

        [DataMember]
        public Int16? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class UnsignedShortColumnValue : ColumnValue
    {
        public UnsignedShortColumnValue()
        {
            this.DataType = DataType.UnsignedShort;
        }

        [DataMember]
        public UInt16? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class UnsignedIntegerColumnValue : ColumnValue
    {
        public UnsignedIntegerColumnValue()
        {
            this.DataType = DataType.UnsignedInteger;
        }

        [DataMember]
        public UInt32? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class LongColumnValue : ColumnValue
    {
        public LongColumnValue()
        {
            this.DataType = DataType.Long;
        }

        [DataMember]
        public Int64? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class UnsignedLongColumnValue : ColumnValue
    {
        public UnsignedLongColumnValue()
        {
            this.DataType = DataType.UnsignedLong;
        }

        [DataMember]
        public UInt64? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class SignedByteColumnValue : ColumnValue
    {
        public SignedByteColumnValue()
        {
            this.DataType = DataType.SignedByte;
        }

        [DataMember]
        public sbyte? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class TimeColumnValue : ColumnValue
    {
        public TimeColumnValue()
        {
            this.DataType = DataType.Time;
        }

        [DataMember]
        public TimeSpan? Value { get; set; }
    }


    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DateColumnValue : ColumnValue
    {
        public DateColumnValue()
        {
            this.DataType = DataType.Date;
        }

        [DataMember]
        public DateTime? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DateTimeOffsetColumnValue : ColumnValue
    {
        public DateTimeOffsetColumnValue()
        {
            this.DataType = DataType.DateTimeOffset;
        }

        [DataMember]
        public DateTimeOffset? Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class XmlColumnValue : ColumnValue
    {
        public XmlColumnValue()
        {
            this.DataType = DataType.Xml;
        }

        [DataMember]
        public string Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class JsonColumnValue : ColumnValue
    {
        public JsonColumnValue()
        {
            this.DataType = DataType.Json;
        }

        [DataMember]
        public string Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ClrEnumColumnValue : ColumnValue
    {
        public ClrEnumColumnValue()
        {
            this.DataType = DataType.ClrEnum;
        }

        [DataMember]
        public Enum Value { get; set; }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ClrTypeColumnValue : ColumnValue
    {
        public ClrTypeColumnValue()
        {
            this.DataType = DataType.ClrType;
        }

        [DataMember]
        public Object Value { get; set; }
    }

    public static class ColumnValueExtensions
    {
        public static object GetValue(this ColumnValue column)
        {
            switch (column.DataType)
            {
                case DataType.String:
                    return ((StringColumnValue)column).Value;
                case DataType.Boolean:
                    return ((BooleanColumnValue)column).Value;
                case DataType.Integer:
                    return ((IntegerColumnValue)column).Value;
                case DataType.DateTime:
                    return ((DateTimeColumnValue)column).Value;
                case DataType.Double:
                    return ((DoubleColumnValue)column).Value;
                case DataType.Float:
                    return ((FloatColumnValue)column).Value;
                case DataType.Decimal:
                    return ((DecimalColumnValue)column).Value;
                case DataType.Byte:
                    return ((ByteColumnValue)column).Value;
                case DataType.Bytes:
                    return ((BytesColumnValue)column).Value;
                case DataType.Guid:
                    return ((GuidColumnValue)column).Value;
                case DataType.Char:
                    return ((CharColumnValue)column).Value;
                case DataType.Short:
                    return ((ShortColumnValue)column).Value;
                case DataType.UnsignedShort:
                    return ((UnsignedShortColumnValue)column).Value;
                case DataType.UnsignedInteger:
                    return ((UnsignedIntegerColumnValue)column).Value;
                case DataType.Long:
                    return ((LongColumnValue)column).Value;
                case DataType.UnsignedLong:
                    return ((UnsignedLongColumnValue)column).Value;
                case DataType.SignedByte:
                    return ((SignedByteColumnValue)column).Value;
                case DataType.Time:
                    return ((TimeColumnValue)column).Value;
                case DataType.Date:
                    return ((DateColumnValue)column).Value;
                case DataType.DateTimeOffset:
                    return ((DateTimeOffsetColumnValue)column).Value;
                case DataType.Xml:
                    throw new InvalidOperationException($"Unsupported data type with name '{column.DataType}'");
                case DataType.Json:
                    throw new InvalidOperationException($"Unsupported data type with name '{column.DataType}'");
                case DataType.ClrEnum:
                    throw new InvalidOperationException($"Unsupported data type with name '{column.DataType}'");
                case DataType.ClrType:
                    throw new InvalidOperationException($"Unsupported data type with name '{column.DataType}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{column.DataType}'");
            }
        }
    }
}
