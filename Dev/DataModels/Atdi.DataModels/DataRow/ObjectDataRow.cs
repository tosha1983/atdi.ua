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
                    result = new BooleanColumnValue
                    {
                        Value = (bool?)value
                    };
                    break;
                case DataType.Integer:
                    result = new IntegerColumnValue
                    {
                        Value = (int?)value
                    };
                    break;
                case DataType.DateTime:
                    result = new DateTimeColumnValue
                    {
                        Value = (DateTime?)value
                    };
                    break;
                case DataType.Double:
                    result = new DoubleColumnValue
                    {
                        Value = (double?)value
                    };
                    break;
                case DataType.Float:
                    result = new FloatColumnValue
                    {
                        Value = (float?)value
                    };
                    break;
                case DataType.Decimal:
                    result = new DecimalColumnValue
                    {
                        Value = (decimal?)value
                    };
                    break;
                case DataType.Byte:
                    result = new ByteColumnValue
                    {
                        Value = (byte?)value
                    };
                    break;
                case DataType.Bytes:
                    result = new BytesColumnValue
                    {
                        Value = (byte[])value
                    };
                    break;
                case DataType.Guid:
                    result = new GuidColumnValue
                    {
                        Value = (Guid?)value
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
