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


        [DataMember]
        public Char?[] CharCells { get; set; }

        [DataMember]
        public Int16?[] ShortCells { get; set; }

        [DataMember]
        public UInt16?[] UnsignedShortCells { get; set; }

        [DataMember]
        public UInt32?[] UnsignedIntegerCells { get; set; }

        [DataMember]
        public Int64?[] LongCells { get; set; }

        [DataMember]
        public UInt64?[] UnsignedLongCells { get; set; }

        [DataMember]
        public sbyte?[] SignedByteCells { get; set; }

        [DataMember]
        public TimeSpan?[] TimeCells { get; set; }

        [DataMember]
        public DateTime?[] DateCells { get; set; }

        [DataMember]
        public DateTimeOffset?[] DateTimeOffsetCells { get; set; }

        [DataMember]
        public string[] XmlCells { get; set; }

        [DataMember]
        public string[] JsonCells { get; set; }

        [DataMember]
        public Enum[] ClrEnumCells { get; set; }

        [DataMember]
        public Object[] ClrTypeCells { get; set; }

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

                case DataType.Char:
                    result = new CharColumnValue
                    {
                        Value = row.CharCells[index]
                    };
                    break;
                case DataType.Short:
                    result = new ShortColumnValue
                    {
                        Value = row.ShortCells[index]
                    };
                    break;
                case DataType.UnsignedShort:
                    result = new UnsignedShortColumnValue
                    {
                        Value = row.UnsignedShortCells[index]
                    };
                    break;
                case DataType.UnsignedInteger:
                    result = new UnsignedIntegerColumnValue
                    {
                        Value = row.UnsignedIntegerCells[index]
                    };
                    break;
                case DataType.Long:
                    result = new LongColumnValue
                    {
                        Value = row.LongCells[index]
                    };
                    break;
                case DataType.UnsignedLong:
                    result = new UnsignedLongColumnValue
                    {
                        Value = row.UnsignedLongCells[index]
                    };
                    break;
                case DataType.SignedByte:
                    result = new SignedByteColumnValue
                    {
                        Value = row.SignedByteCells[index]
                    };
                    break;
                case DataType.Time:
                    result = new TimeColumnValue
                    {
                        Value = row.TimeCells[index]
                    };
                    break;
                case DataType.Date:
                    result = new DateColumnValue
                    {
                        Value = row.DateCells[index]
                    };
                    break;
                case DataType.DateTimeOffset:
                    result = new DateTimeOffsetColumnValue
                    {
                        Value = row.DateTimeOffsetCells[index]
                    };
                    break;
                case DataType.Xml:
                    result = new XmlColumnValue
                    {
                        Value = row.XmlCells[index]
                    };
                    break;
                case DataType.Json:
                    result = new JsonColumnValue
                    {
                        Value = row.JsonCells[index]
                    };
                    break;
                case DataType.ClrEnum:
                    result = new ClrEnumColumnValue
                    {
                        Value = row.ClrEnumCells[index]
                    };
                    break;
                case DataType.ClrType:
                    result = new ClrTypeColumnValue
                    {
                        Value = row.ClrTypeCells[index]
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
