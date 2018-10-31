using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels
{
    /// <summary>
    /// Represents the data of the result of the executed query
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class TypedCellsDataSet : DataSet
    {
        public TypedCellsDataSet()
        {
            this.Structure = DataSetStructure.TypedCells;
        }

        [DataMember]
        public string[][] StringCells { get; set; }

        [DataMember]
        public bool?[][] BooleanCells { get; set; }

        [DataMember]
        public DateTime?[][] DateTimeCells { get; set; }

        [DataMember]
        public int?[][] IntegerCells { get; set; }

        [DataMember]
        public double?[][] DoubleCells { get; set; }

        [DataMember]
        public Single?[][] FloatCells { get; set; }

        [DataMember]
        public decimal?[][] DecimalCells { get; set; }

        [DataMember]
        public byte?[][] ByteCells { get; set; }

        [DataMember]
        public byte[][][] BytesCells { get; set; }

        [DataMember]
        public Guid?[][] GuidCells { get; set; }

        [DataMember]
        public Char?[][] CharCells { get; set; }

        [DataMember]
        public Int16?[][] ShortCells { get; set; }

        [DataMember]
        public UInt16?[][] UnsignedShortCells { get; set; }

        [DataMember]
        public UInt32?[][] UnsignedIntegerCells { get; set; }

        [DataMember]
        public Int64?[][] LongCells { get; set; }

        [DataMember]
        public UInt64?[][] UnsignedLongCells { get; set; }

        [DataMember]
        public sbyte?[][][] SignedByteCells { get; set; }

        [DataMember]
        public TimeSpan?[][] TimeCells { get; set; }

        [DataMember]
        public DateTime?[][] DateCells { get; set; }

        [DataMember]
        public DateTimeOffset?[][] DateTimeOffsetCells { get; set; }

        [DataMember]
        public string[][] XmlCells { get; set; }

        [DataMember]
        public string[][] JsonCells { get; set; }

        [DataMember]
        public Enum[][] ClrEnumCells { get; set; }

        [DataMember]
        public Object[][] ClrTypeCells { get; set; }
    }
}
