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
    public class ObjectRowsDataSet : DataSet
    {
        public ObjectRowsDataSet()
        {
            this.Structure = DataSetStructure.ObjectRows;
        }

        [DataMember]
        public ObjectDataRow[] Rows { get; set; }

    }
}
