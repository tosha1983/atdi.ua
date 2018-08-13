using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.DataConstraint;
using Atdi.Common;

namespace Atdi.DataModels
{
    /// <summary>
    /// Represents the metadata to the column
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class StringDataRow
    {
        [DataMember]
        public string[] Cells { get; set; }
    }

    public static class StringDataRowExtensions
    {
        public static IFormatProvider CultureEnUs = new System.Globalization.CultureInfo("en-US");
        public static ColumnValue GetColumnValue(this StringDataRow row, DataSetColumn dataSetColumn)
        {
            var result = row.GetColumnValue(dataSetColumn.Type, dataSetColumn.Index);
            result.Name = dataSetColumn.Name;
            return result;
        }

        public static ColumnValue GetColumnValue(this StringDataRow row, DataType dataType, int index)
        {
            ColumnValue result = null;
            var value = row.Cells[index];
            switch (dataType)
            {
                case DataType.String:
                    result = new StringColumnValue
                    {
                        Value = value ?? null
                    };
                    break;
                case DataType.Boolean:
                    bool valueBool = false;    int valueInt = 0;
                    if (bool.TryParse(value, out valueBool))
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : valueBool as bool?
                        };
                    }
                    else if (int.TryParse(value, out valueInt))
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
                         Value = (value == null) ? (int?)null : (int.Parse(value, CultureEnUs) as int?)
                    };
                    break;
                case DataType.DateTime:
                    result = new DateTimeColumnValue
                    {
                         Value = (value == null) ? (DateTime?)null : value.ConvertISO8601ToDateTime()
                    };
                    break;
                case DataType.Double:
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value, CultureEnUs) as double?)
                    };
                    break;
                case DataType.Float:
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value, CultureEnUs) as float?)
                    };
                    break;
                case DataType.Decimal:
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value, CultureEnUs) as decimal?)
                    };
                    break;
                case DataType.Byte:
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : Convert.ToByte(value) as byte ?
                    };
                    break;
                case DataType.Bytes:
                    result = new BytesColumnValue
                    {
                        Value = (value == null) ? (byte[])null : UTF8Encoding.UTF8.GetBytes(value)
                    };
                    break;
                case DataType.Guid:
                    result = new GuidColumnValue
                    {
                         Value = (value == null) ? (Guid?)null : (Guid.Parse(value) as Guid?)
                    };
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{dataType}'");
            }
            return result;
        }

        public static ColumnValue[] GetColumnsValues(this StringDataRow row, DataSetColumn[] dataSetColumns)
        {
            var resultColumns = new ColumnValue[dataSetColumns.Length];
            for (int i = 0; i < dataSetColumns.Length; i++)
            {
                resultColumns[i] = row.GetColumnValue(dataSetColumns[i]);
            }
            return resultColumns;
        }
    }
}
