using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.DataConstraint;

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
            var ci = new System.Globalization.CultureInfo("en-US");
            switch (dataType)
            {
                case DataType.String:
                    result = new StringColumnValue
                    {
                        Value = value ?? null
                    };
                    break;
                case DataType.Boolean:
                    try
                    {
                        result = new BooleanColumnValue
                        {
                            Value = (value == null) ? (bool?)null : bool.Parse(value) as bool?
                        };
                    }
                    catch (Exception)
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
                         Value = (value == null) ? (int?)null : (int.Parse(value, ci) as int?)
                    };
                    break;
                case DataType.DateTime:
                    try
                    {
                        result = new DateTimeColumnValue
                        {
                            Value = (value == null) ? (DateTime?)null : value.ConvertISO8601ToDateTime()
                        };
                    }
                    catch (Exception)
                    {
                        var formats = new[] { "M-d-yyyy", "dd-MM-yyyy", "MM-dd-yyyy", "M.d.yyyy", "dd.MM.yyyy", "MM.dd.yyyy", "dd.MM.yyyy H:mm:ss" };
                        DateTime? DT_Val_DATA_CERERE = null; try { DT_Val_DATA_CERERE = (DateTime.ParseExact(value.ToString(), formats, ci, System.Globalization.DateTimeStyles.AssumeLocal)); }
                        catch (Exception) { }
                        result = new DateTimeColumnValue
                        {
                            Value = DT_Val_DATA_CERERE
                        };
                    }
                    break;
                case DataType.Double:
                    result = new DoubleColumnValue
                    {
                        Value = (value == null) ? (double?)null : (double.Parse(value, ci) as double?)
                    };
                    break;
                case DataType.Float:
                    result = new FloatColumnValue
                    {
                        Value = (value == null) ? (float?)null : (float.Parse(value, ci) as float?)
                    };
                    break;
                case DataType.Decimal:
                    result = new DecimalColumnValue
                    {
                        Value = (value == null) ? (decimal?)null : (decimal.Parse(value, ci) as decimal?)
                    };
                    break;
                case DataType.Byte:
                    result = new ByteColumnValue
                    {
                        Value = (value == null) ? (byte?)null : UTF8Encoding.UTF8.GetBytes(value)[0] as byte?
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

        private static DateTime ConvertISO8601ToDateTime(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new System.ArgumentNullException("value");
            }
            return new System.DateTime(System.Convert.ToInt32(value.Substring(0, 4)), System.Convert.ToInt32(value.Substring(5, 2)), System.Convert.ToInt32(value.Substring(8, 2)), System.Convert.ToInt32(value.Substring(11, 2)), System.Convert.ToInt32(value.Substring(14, 2)), System.Convert.ToInt32(value.Substring(17, 2)), new System.Globalization.GregorianCalendar());
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
