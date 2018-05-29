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
    public class ObjectRowCreationAction : CreationAction
    {
        public ObjectRowCreationAction()
        {
            this.Type = ActionType.Create;
            this.RowType = DataRowType.ObjectCell;
        }

        [DataMember]
        public ObjectDataRow Row { get; set; }
    }
}
