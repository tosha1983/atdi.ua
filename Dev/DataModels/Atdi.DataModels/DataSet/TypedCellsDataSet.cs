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
        public float?[][] FloatCells { get; set; }

        [DataMember]
        public decimal?[][] DecimalCells { get; set; }

        [DataMember]
        public byte?[][] ByteCells { get; set; }

        [DataMember]
        public byte[][][] BytesCells { get; set; }

        [DataMember]
        public Guid?[][] GuidCells { get; set; }
    }
}
