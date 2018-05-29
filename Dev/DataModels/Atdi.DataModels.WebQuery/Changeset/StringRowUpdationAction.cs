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
    public class StringRowUpdationAction : UpdationAction
    {
        public StringRowUpdationAction()
        {
            this.Type = ActionType.Update;
            this.RowType = DataRowType.StringCell;
        }

        [DataMember]
        public StringDataRow Row { get; set; }
        
    }
}
