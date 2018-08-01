using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.DataConstraint;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Atdi.Common;

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
                        Value = (value == null) ? (DateTime?)null : value.ToString().ConvertISO8601ToDateTime()
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
                        Value = float.Parse(value.ToString()) as float?
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
                        Value = (value == null) ? (byte?)null : UTF8Encoding.UTF8.GetBytes(value.ToString())[0] as byte?
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
    }
}
