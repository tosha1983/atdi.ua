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
    public class StringRowsDataSet : DataSet
    {
        public StringRowsDataSet()
        {
            this.Structure = DataSetStructure.StringRows;
        }

        [DataMember]
        public StringDataRow[] Rows { get; set; }

    }
}
