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
    public class TypedDataRow
    {
        [DataMember]
        public string[] StringCells { get; set; }

        [DataMember]
        public bool?[] BooleanCells { get; set; }

        [DataMember]
        public DateTime?[] DateTimeCells { get; set; }

        [DataMember]
        public int?[] IntegerCells { get; set; }

        [DataMember]
        public double?[] DoubleCells { get; set; }

        [DataMember]
        public float?[] FloatCells { get; set; }

        [DataMember]
        public decimal?[] DecimalCells { get; set; }

        [DataMember]
        public byte?[] ByteCells { get; set; }

        [DataMember]
        public byte[][] BytesCells { get; set; }

        [DataMember]
        public Guid?[] GuidCells { get; set; }
    }

    public static class TypedDataRowExtensions
    {
        public static ColumnValue GetColumnValue(this TypedDataRow row, DataSetColumn dataSetColumn)
        {
            var result = row.GetColumnValue(dataSetColumn.Type, dataSetColumn.Index);
            result.Name = dataSetColumn.Name;
            return result;
        }

        public static ColumnValue GetColumnValue(this TypedDataRow row, DataType dataType, int index)
        {
            ColumnValue result = null;
            switch (dataType)
            {
                case DataType.String:
                    result = new StringColumnValue
                    {
                        Value = row.StringCells[index]
                    };
                    break;
                case DataType.Boolean:
                    result = new BooleanColumnValue
                    {
                        Value = row.BooleanCells[index]
                    };
                    break;
                case DataType.Integer:
                    result = new IntegerColumnValue
                    {
                        Value = row.IntegerCells[index]
                    };
                    break;
                case DataType.DateTime:
                    result = new DateTimeColumnValue
                    {
                        Value = row.DateTimeCells[index]
                    };
                    break;
                case DataType.Double:
                    result = new DoubleColumnValue
                    {
                        Value = row.DoubleCells[index]
                    };
                    break;
                case DataType.Float:
                    result = new FloatColumnValue
                    {
                        Value = row.FloatCells[index]
                    };
                    break;
                case DataType.Decimal:
                    result = new DecimalColumnValue
                    {
                        Value = row.DecimalCells[index]
                    };
                    break;
                case DataType.Byte:
                    result = new ByteColumnValue
                    {
                        Value = row.ByteCells[index]
                    };
                    break;
                case DataType.Bytes:
                    result = new BytesColumnValue
                    {
                        Value = row.BytesCells[index]
                    };
                    break;
                case DataType.Guid:
                    result = new GuidColumnValue
                    {
                        Value = row.GuidCells[index]
                    };
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{dataType}'");
            }
            return result;
        }

        public static ColumnValue[] GetColumnsValues(this TypedDataRow row, DataSetColumn[] dataSetColumns)
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
