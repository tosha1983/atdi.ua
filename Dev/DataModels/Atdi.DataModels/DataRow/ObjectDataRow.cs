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
}
