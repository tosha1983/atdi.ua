using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.DataConstraint;

namespace Atdi.DataModels.WebQuery
{
    // <summary>
    /// Represents the action of update record
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class TypedRowUpdationAction : UpdationAction
    {
        public TypedRowUpdationAction()
        {
            this.Type = ActionType.Update;
            this.RowType = DataRowType.TypedCell;
        }

        [DataMember]
        public TypedDataRow Row { get; set; }
        
    }
}
