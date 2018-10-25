using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.DataConstraint;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace Atdi.DataModels
{
    /// <summary>
    /// Represents the metadata to the column
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ObjectDataRow
    {
        [DataMember]
        public object[] Cells{ get; set; }
    }

    public static class ObjectDataRowExtensions
    {
        public static ColumnValue GetColumnValue(this ObjectDataRow row, DataSetColumn dataSetColumn)
        {
            var result = row.GetColumnValue(dataSetColumn.Type, dataSetColumn.Index);
            result.Name = dataSetColumn.Name;
            return result;
        }

        public static ColumnValue GetColumnValue(this ObjectDataRow row, DataType dataType, int index)
        {
            ColumnValue result = null;
            var value = row.Cells[index];
            switch (dataType)
            {
                case DataType.String:
                    result = new StringColumnValue
                    {
                        Value = (string)value
                    };
                    break;
                case DataType.Boolean:
                    bool valueBool = false; int valueInt = 0;
                    if (bool.TryParse(value.ToString(), out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?
                        };
                    }
                    else if (int.TryParse(value.ToString(), out valueInt))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = value.ToString() == "1" ? (bool?)true : (bool?)false
                        };
                    }
                    break;
                case DataType.Integer:
                    result = new IntegerColumnValue
                    {
                        Value = (value == null) ? (int?)null : Convert.ToInt32(value)
                    };
                    break;
                case DataType.DateTime:
                    result = new DateTimeColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind)
                    };
                    break;
                case DataType.Double:
                    result = new DoubleColumnValue
                    {
                        Value = double.Parse(value.ToString()) as double?
                    };
                    break;
                case DataType.Float:
                    result = new FloatColumnValue
                    {
                        Value = Single.Parse(value.ToString()) as Single?
                    };
                    break;
                case DataType.Decimal:
                    result = new DecimalColumnValue
                    {
                        Value = decimal.Parse(value.ToString()) as decimal?
                    };
                    break;
                case DataType.Byte:
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte?
                    };
                    break;
                case DataType.Bytes:
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : UTF8Encoding.UTF8.GetBytes(value.ToString())
                    };
                    break;
                case DataType.Guid:
                    result = new GuidColumnValue
                    {
                        Value = Guid.Parse(value.ToString()) as Guid?
                    };
                    break;
                case DataType.Char:
                    result = new CharColumnValue
                    {
                        Value = (value == null) ? (char?)null : Convert.ToChar(value) as char ?
                    };
                    break;
                case DataType.Short:
                    result = new ShortColumnValue
                    {
                        Value = (value == null) ? (Int16?)null : value as Int16?
                    };
                    break;
                case DataType.UnsignedShort:
                    result = new UnsignedShortColumnValue
                    {
                        Value = (value == null) ? (UInt16?)null : value as UInt16?
                    };
                    break;
                case DataType.UnsignedInteger:
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = (value == null) ? (UInt32?)null : value as UInt32?
                    };
                    break;
                case DataType.Long:
                    result = new LongColumnValue
                    {
                        Value = (value == null) ? (Int64?)null : value as Int64?
                    };
                    break;
                case DataType.UnsignedLong:
                    result = new UnsignedLongColumnValue
                    {
                        Value = (value == null) ? (UInt64?)null : value as UInt64?
                    };
                    break;
                case DataType.SignedByte:
                    result = new SignedByteColumnValue
                    {
                        Value = (value == null) ? (sbyte?)null : value as sbyte?
                    };
                    break;
                case DataType.Time:
                    result = new TimeColumnValue
                    {
                        Value = (value == null) ? (TimeSpan?)null : value as TimeSpan?
                    };
                    break;
                case DataType.Date:
                    result = new DateColumnValue
                    {
                        Value = (value == null) ? (DateTime?)null : DateTime.Parse(value.ToString(), null, System.Globalization.DateTimeStyles.RoundtripKind)
                    };
                    break;
                case DataType.DateTimeOffset:
                    result = new DateTimeOffsetColumnValue
                    {
                        Value = (value == null) ? (DateTimeOffset?)null : DateTimeOffset.Parse(value.ToString()) as DateTimeOffset?
                    };
                    break;
                case DataType.Xml:
                    throw new InvalidOperationException($"Unsupported data type with name '{dataType}'");
                case DataType.Json:
                    throw new InvalidOperationException($"Unsupported data type with name '{dataType}'");
                case DataType.ClrEnum:
                    throw new InvalidOperationException($"Unsupported data type with name '{dataType}'");
                case DataType.ClrType:
                    throw new InvalidOperationException($"Unsupported data type with name '{dataType}'");
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{dataType}'");
            }
            return result;
        }


        public static ColumnValue[] GetColumnsValues(this ObjectDataRow row, DataSetColumn[] dataSetColumns)
        {
            var resultColumns = new ColumnValue[dataSetColumns.Length];
            for (int i = 0; i < dataSetColumns.Length; i++)
            {
                resultColumns[i] = row.GetColumnValue(dataSetColumns[i]);
            }
            return resultColumns;
        }

        public static T ToEnum<T>(string enumString)
        {
            return (T)Enum.Parse(typeof(T), enumString);
        }
    }
}
