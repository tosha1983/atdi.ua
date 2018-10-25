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
                switch (filterOperand.DataType)
                {
                    case DataType.Undefined:
                        return new ValueOperand { Type = OperandType.Value, DataType = DataType.Undefined };
                    case DataType.String:
                        return new StringValueOperand { Type = OperandType.Value, DataType = DataType.String, Value = Convert.ToString(filterOperand.Value) };
                    case DataType.Boolean:
                        return new BooleanValueOperand { Type = OperandType.Value, DataType = DataType.Boolean, Value = (filterOperand.Value == null) ? (bool?)null : Convert.ToBoolean(filterOperand.Value) };
                    case DataType.Integer:
                        return new IntegerValueOperand { Type = OperandType.Value, DataType = DataType.Integer, Value = (filterOperand.Value == null) ? (int?)null : Convert.ToInt32(filterOperand.Value) };
                    case DataType.DateTime:
                        return new DateTimeValueOperand { Type = OperandType.Value, DataType = DataType.DateTime , Value = (filterOperand.Value == null) ? (DateTime?)null : Convert.ToDateTime(filterOperand.Value) };
                    case DataType.Double:
                        return new DoubleValueOperand { Type = OperandType.Value, DataType = DataType.Double , Value = (filterOperand.Value == null) ? (double?)null : Convert.ToDouble(filterOperand.Value) };
                    case DataType.Float:
                        return new FloatValueOperand { Type = OperandType.Value, DataType = DataType.Float, Value = (filterOperand.Value == null) ? (Single?)null : (Single)Convert.ToDouble(filterOperand.Value) };
                    case DataType.Decimal:
                        return new DecimalValueOperand { Type = OperandType.Value, DataType = DataType.Decimal, Value = (filterOperand.Value == null) ? (decimal?)null : Convert.ToDecimal(filterOperand.Value) };
                    case DataType.Byte:
                        return new ByteValueOperand { Type = OperandType.Value, DataType = DataType.Byte, Value = (filterOperand.Value == null) ? (byte?)null : Convert.ToByte(filterOperand.Value) };
                    case DataType.Bytes:
                        return new BytesValueOperand { Type = OperandType.Value, DataType = DataType.Bytes, Value = (filterOperand.Value == null) ? (byte[])null : (byte[])(filterOperand.Value) };
                    case DataType.Guid:
                        return new GuidValueOperand { Type = OperandType.Value, DataType = DataType.Guid, Value = (filterOperand.Value == null) ? (Guid?)null : (Guid)(filterOperand.Value) };
                    case DataType.Char:
                        return new CharValueOperand { Type = OperandType.Value, DataType = DataType.Char, Value = (filterOperand.Value == null) ? (char?)null : (char)(filterOperand.Value)};
                    case DataType.Short:
                        return new ShortValueOperand { Type = OperandType.Value, DataType = DataType.Short, Value = (filterOperand.Value == null) ? (Int16?)null : (Int16)(filterOperand.Value) };
                    case DataType.UnsignedShort:
                        return new UnsignedShortValueOperand { Type = OperandType.Value, DataType = DataType.UnsignedShort, Value = (filterOperand.Value == null) ? (UInt16?)null : (UInt16)(filterOperand.Value) };
                    case DataType.UnsignedInteger:
                        return new UnsignedIntegerValueOperand { Type = OperandType.Value, DataType = DataType.UnsignedInteger, Value = (filterOperand.Value == null) ? (UInt32?)null : (UInt32)(filterOperand.Value) };
                    case DataType.Long:
                        return new LongValueOperand { Type = OperandType.Value, DataType = DataType.Long, Value = (filterOperand.Value == null) ? (Int64?)null : (Int64)(filterOperand.Value) };
                    case DataType.UnsignedLong:
                        return new UnsignedLongValueOperand { Type = OperandType.Value, DataType = DataType.UnsignedLong, Value = (filterOperand.Value == null) ? (UInt64?)null : (UInt64)(filterOperand.Value) };
                    case DataType.SignedByte:
                        return new SignedByteValueOperand { Type = OperandType.Value, DataType = DataType.SignedByte, Value = (filterOperand.Value == null) ? (sbyte?)null : (sbyte)(filterOperand.Value) };
                    case DataType.Time:
                        return new TimeValueOperand { Type = OperandType.Value, DataType = DataType.Time, Value = (filterOperand.Value == null) ? (TimeSpan?)null : (TimeSpan)(filterOperand.Value) };
                    case DataType.Date:
                        return new DateValueOperand { Type = OperandType.Value, DataType = DataType.Date, Value = (filterOperand.Value == null) ? (DateTime?)null : (DateTime)(filterOperand.Value) };
                    case DataType.DateTimeOffset:
                        return new DateTimeOffsetValueOperand { Type = OperandType.Value, DataType = DataType.DateTimeOffset, Value = (filterOperand.Value == null) ? (DateTimeOffset?)null : (DateTimeOffset)(filterOperand.Value) };
                    case DataType.Xml:
                        throw new InvalidOperationException($"Unsupported the data type with name '{filterOperand.DataType}'");
                    case DataType.Json:
                        throw new InvalidOperationException($"Unsupported the data type with name '{filterOperand.DataType}'");
                    case DataType.ClrEnum:
                        throw new InvalidOperationException($"Unsupported the data type with name '{filterOperand.DataType}'");
                    case DataType.ClrType:
                        throw new InvalidOperationException($"Unsupported the data type with name '{filterOperand.DataType}'");

                    default:
                        throw new InvalidOperationException($"Unsupported the data type with name '{filterOperand.DataType}'");
                }
            }

            if (filterOperand.Type == OperandType.Values)
            {
                switch (filterOperand.DataType)
                {
                    case DataType.Undefined:
                        return new ValuesOperand { Type = OperandType.Value, DataType = DataType.Undefined };
                    case DataType.String:
                        return new StringValuesOperand { Type = OperandType.Value, DataType = DataType.String, Values = filterOperand.Values?.Select(v => (v == null) ? null : Convert.ToString(v)).ToArray() };
                    case DataType.Boolean:
                        return new BooleanValuesOperand { Type = OperandType.Value, DataType = DataType.Boolean, Values = filterOperand.Values?.Select(v => (v == null) ? (bool?) null : Convert.ToBoolean(v)).ToArray() };
                    case DataType.Integer:
                        return new IntegerValuesOperand { Type = OperandType.Value, DataType = DataType.Integer, Values = filterOperand.Values?.Select(v => (v == null) ? (int?)null : Convert.ToInt32(v)).ToArray() };
                    case DataType.DateTime:
                        return new DateTimeValuesOperand { Type = OperandType.Value, DataType = DataType.DateTime, Values = filterOperand.Values?.Select(v => (v == null) ? (DateTime?)null : Convert.ToDateTime(v)).ToArray() };
                    case DataType.Double:
                        return new DoubleValuesOperand { Type = OperandType.Value, DataType = DataType.Double, Values = filterOperand.Values?.Select(v => (v == null) ? (double?)null : Convert.ToDouble(v)).ToArray() };
                    case DataType.Float:
                        return new FloatValuesOperand { Type = OperandType.Value, DataType = DataType.Float, Values = filterOperand.Values?.Select(v => (v == null) ? (float?)null : (float)Convert.ToDouble(v)).ToArray() };
                    case DataType.Decimal:
                        return new DecimalValuesOperand { Type = OperandType.Value, DataType = DataType.Decimal, Values = filterOperand.Values?.Select(v => (v == null) ? (decimal?)null : Convert.ToDecimal(v)).ToArray() };
                    case DataType.Byte:
                        return new ByteValuesOperand { Type = OperandType.Value, DataType = DataType.Byte, Values = filterOperand.Values?.Select(v => (v == null) ? (byte?)null : Convert.ToByte(v)).ToArray() };
                    case DataType.Bytes:
                        return new BytesValuesOperand { Type = OperandType.Value, DataType = DataType.Bytes, Values = filterOperand.Values?.Select(v => (v == null) ? (byte[])null : (byte[])(v)).ToArray() };
                    case DataType.Guid:
                        return new GuidValuesOperand { Type = OperandType.Value, DataType = DataType.Guid, Values = filterOperand.Values?.Select(v => (v == null) ? (Guid?)null : (Guid)(v)).ToArray() }; ;
                    case DataType.Char:
                        return new CharValuesOperand { Type = OperandType.Value, DataType = DataType.Char, Values = filterOperand.Values?.Select(v => (v == null) ? (char?)null : Convert.ToChar(v)).ToArray() };
                    case DataType.Short:
                        return new ShortValuesOperand { Type = OperandType.Value, DataType = DataType.Short, Values = filterOperand.Values?.Select(v => (v == null) ? (Int16?)null : (Int16)(v)).ToArray() };
                    case DataType.UnsignedShort:
                        return new UnsignedShortValuesOperand { Type = OperandType.Value, DataType = DataType.UnsignedShort, Values = filterOperand.Values?.Select(v => (v == null) ? (UInt16?)null : (UInt16)(v)).ToArray() };
                    case DataType.UnsignedInteger:
                        return new UnsignedIntegerValuesOperand { Type = OperandType.Value, DataType = DataType.UnsignedInteger, Values = filterOperand.Values?.Select(v => (v == null) ? (UInt32?)null : (UInt32)(v)).ToArray() };
                    case DataType.Long:
                        return new LongValuesOperand { Type = OperandType.Value, DataType = DataType.Long, Values = filterOperand.Values?.Select(v => (v == null) ? (Int64?)null : (Int64)(v)).ToArray() };
                    case DataType.UnsignedLong:
                        return new UnsignedLongValuesOperand { Type = OperandType.Value, DataType = DataType.UnsignedLong, Values = filterOperand.Values?.Select(v => (v == null) ? (UInt64?)null : (UInt64)(v)).ToArray() };
                    case DataType.SignedByte:
                        return new SignedByteValuesOperand { Type = OperandType.Value, DataType = DataType.SignedByte, Values = filterOperand.Values?.Select(v => (v == null) ? (sbyte?)null : (sbyte)(v)).ToArray() };
                    case DataType.Time:
                        return new TimeValuesOperand { Type = OperandType.Value, DataType = DataType.Time, Values = filterOperand.Values?.Select(v => (v == null) ? (TimeSpan?)null : (TimeSpan)(v)).ToArray() };
                    case DataType.Date:
                        return new DateValuesOperand { Type = OperandType.Value, DataType = DataType.Date, Values = filterOperand.Values?.Select(v => (v == null) ? (DateTime?)null : (DateTime)(v)).ToArray() };
                    case DataType.DateTimeOffset:
                        return new DateTimeOffsetValuesOperand { Type = OperandType.Value, DataType = DataType.DateTimeOffset, Values = filterOperand.Values?.Select(v => (v == null) ? (DateTimeOffset?)null : (DateTimeOffset)(v)).ToArray() };
                    case DataType.Xml:
                        throw new InvalidOperationException($"Unsupported the data type with name '{filterOperand.DataType}'");
                    case DataType.Json:
                        throw new InvalidOperationException($"Unsupported the data type with name '{filterOperand.DataType}'");
                    case DataType.ClrEnum:
                        throw new InvalidOperationException($"Unsupported the data type with name '{filterOperand.DataType}'");
                    case DataType.ClrType:
                        throw new InvalidOperationException($"Unsupported the data type with name '{filterOperand.DataType}'");

                    default:
                        throw new InvalidOperationException($"Unsupported the data type with name '{filterOperand.DataType}'");
                }
            }

            throw new InvalidOperationException($"Unsupported the filter operand type with name '{filterOperand.Type}'");
        }
    }
}
