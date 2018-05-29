using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.WebQuery
{
    // <summary>
    /// Represents the action of create record
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class StringRowCreationAction : CreationAction
    {
        public StringRowCreationAction()
        {
            this.Type = ActionType.Create;
            this.RowType = DataRowType.StringCell;
        }

        [DataMember]
        public StringDataRow Row { get; set; }
    }
}
