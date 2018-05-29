using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels
{
    /// <summary>
    /// Represents the set of data
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    [KnownType(typeof(TypedCellsDataSet))]
    [KnownType(typeof(StringCellsDataSet))]
    [KnownType(typeof(ObjectCellsDataSet))]
    [KnownType(typeof(TypedRowsDataSet))]
    [KnownType(typeof(StringRowsDataSet))]
    [KnownType(typeof(ObjectRowsDataSet))]
    public class DataSet
    {
        [DataMember]
        public DataSetColumn[] Columns { get; set; }

        [DataMember]
        public DataSetStructure Structure { get; set; }

        [DataMember]
        public int RowCount { get; set; }
    }
}
