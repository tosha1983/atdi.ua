using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels
{
    // <summary>
    /// Represents the action of create record
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    [KnownType(typeof(TypedRowCreationAction))]
    [KnownType(typeof(StringRowCreationAction))]
    [KnownType(typeof(ObjectRowCreationAction))]
    public class CreationAction : Action
    {
        [DataMember]
        public DataSetColumn[] Columns { get; set; }

        [DataMember]
        public DataRowType RowType { get; set; }
    }
}
