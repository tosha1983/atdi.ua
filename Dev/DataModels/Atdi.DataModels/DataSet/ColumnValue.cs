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
    [KnownType(typeof(CharsColumnValue))]
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

        public static ColumnValue Create(DataType dataType, object value, string name)
        {
            return null;
        }

        public static ColumnValue Create<T>(T value, string name)
        {
            var result = default(ColumnValue);
            var type = typeof(T);

            if (type == typeof(bool) || type == typeof(bool?))
            {
                result = new BooleanColumnValue
                {
                    Value = (bool?)(object)value
                };
            }
            else if (type == typeof(char) || type == typeof(char?))
            {
                result = new CharColumnValue
                {
                    Value = (char?)(object)value
                };
            }
            else if (type == typeof(char[]))
            {
                result = new CharsColumnValue
                {
                    Value = (char[])(object)value
                };
            }
            else if (type == typeof(string))
            {
                result = new StringColumnValue
                {
                    Value = (string)(object)value
                };
            }
            else if (type == typeof(short) || type == typeof(short?))
            {
                result = new ShortColumnValue
                {
                    Value = (short?)(object)value
                };
            }
            else if (type == typeof(ushort) || type == typeof(ushort?))
            {
                result = new UnsignedShortColumnValue
                {
                    Value = (ushort?)(object)value
                };
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                result = new IntegerColumnValue
                {
                    Value = (int?)(object)value
                };
            }
            else if (type == typeof(uint) || type == typeof(uint?))
            {
                result = new UnsignedIntegerColumnValue
                {
                    Value = (uint?)(object)value
                };
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                result = new LongColumnValue
                {
                    Value = (long?)(object)value
                };
            }
            else if (type == typeof(ulong) || type == typeof(ulong?))
            {
                result = new UnsignedLongColumnValue
                {
                    Value = (ulong?)(object)value
                };
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                result = new FloatColumnValue
                {
                    Value = (float?)(object)value
                };
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                result = new DoubleColumnValue
                {
                    Value = (double?)(object)value
                };
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                result = new DecimalColumnValue
                {
                    Value = (decimal?)(object)value
                };
            }
            else if (type == typeof(byte) || type == typeof(byte?))
            {
                result = new ByteColumnValue
                {
                    Value = (byte?)(object)value
                };
            }
            else if (type == typeof(sbyte) || type == typeof(sbyte?))
            {
                result = new SignedByteColumnValue
                {
                    Value = (sbyte?)(object)value
                };
            }
            else if (type == typeof(byte[]))
            {
                result = new BytesColumnValue
                {
                    Value = (byte[])(object)value
                };
            }
            else if (type == typeof(Guid) || type == typeof(Guid?))
            {
                result = new GuidColumnValue
                {
                    Value = (Guid?)(object)value
                };
            }
            else if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
            {
                result = new TimeColumnValue
                {
                    Value = (TimeSpan?)(object)value
                };
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                result = new DateTimeColumnValue
                {
                    Value = (DateTime?)(object)value
                };
            }
            else if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            {
                result = new DateTimeOffsetColumnValue
                {
                    Value = (DateTimeOffset?)(object)value
                };
            }
            else if (type.IsArray)
            {
                result = new ClrTypeColumnValue
                {
                    Value = value
                };
            }
            else
            {
                throw new InvalidCastException($"Unsupported type {type.AssemblyQualifiedName} for creating column value");
            }
            result.Name = name;
            return result;
        }
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

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Value))
            {
                return $"{this.Name}({this.DataType}) is empty";
            }
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
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

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
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

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class IntegerColumnValue : ColumnValue
    {
        public IntegerColumnValue()
        {
            this.DataType = DataType.Integer;
        }

        [DataMember]
        public int? Value { get; set; }

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
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

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
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

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
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

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
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

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
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

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }

            if (this.Value.Length == 0)
            {
                return $"{this.Name}({this.DataType}) is empty";
            }
            return $"{this.Name}({this.DataType}).Length = '{this.Value.Length}'";
        }
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

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class CharColumnValue : ColumnValue
    {
        public CharColumnValue()
        {
            this.DataType = DataType.Char;
        }

        [DataMember]
        public char? Value { get; set; }

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class CharsColumnValue : ColumnValue
    {
        public CharsColumnValue()
        {
            this.DataType = DataType.Chars;
        }

        [DataMember]
        public char[] Value { get; set; }

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ShortColumnValue : ColumnValue
    {
        public ShortColumnValue()
        {
            this.DataType = DataType.Short;
        }

        [DataMember]
        public short? Value { get; set; }

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class UnsignedShortColumnValue : ColumnValue
    {
        public UnsignedShortColumnValue()
        {
            this.DataType = DataType.UnsignedShort;
        }

        [DataMember]
        public ushort? Value { get; set; }

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class UnsignedIntegerColumnValue : ColumnValue
    {
        public UnsignedIntegerColumnValue()
        {
            this.DataType = DataType.UnsignedInteger;
        }

        [DataMember]
        public uint? Value { get; set; }

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class LongColumnValue : ColumnValue
    {
        public LongColumnValue()
        {
            this.DataType = DataType.Long;
        }

        [DataMember]
        public long? Value { get; set; }

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class UnsignedLongColumnValue : ColumnValue
    {
        public UnsignedLongColumnValue()
        {
            this.DataType = DataType.UnsignedLong;
        }

        [DataMember]
        public ulong? Value { get; set; }

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
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

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
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

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
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

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
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

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
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

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
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

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
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

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
    }

    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ClrTypeColumnValue : ColumnValue
    {
        public ClrTypeColumnValue()
        {
            this.DataType = DataType.ClrType;
        }

        [DataMember]
        public object Value { get; set; }

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.Name}({this.DataType}) is null";
            }
            return $"{this.Name}({this.DataType}) = '{this.Value}'";
        }
    }

    public static class ColumnValueExtensions
    {
        public static object GetValue(this ColumnValue column)
        {
            switch (column.DataType)
            {
                case DataType.String:
                    return ((StringColumnValue)column).Value;
                case DataType.Integer:
                    return ((IntegerColumnValue)column).Value;
                case DataType.Float:
                    return ((FloatColumnValue)column).Value;
                case DataType.Double:
                    return ((DoubleColumnValue)column).Value;
                case DataType.Boolean:
                    return ((BooleanColumnValue)column).Value;
                case DataType.DateTime:
                    return ((DateTimeColumnValue)column).Value;
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
                case DataType.Chars:
                    return ((CharsColumnValue)column).Value;
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
                    return ((XmlColumnValue)column).Value;
                case DataType.Json:
                    return ((JsonColumnValue)column).Value;
                case DataType.ClrEnum:
                    return ((ClrEnumColumnValue)column).Value;
                case DataType.ClrType:
                    return ((ClrTypeColumnValue)column).Value; ;
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{column.DataType}'");
            }
        }
    }
}
