using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.DataConstraint;

namespace Atdi.DataModels
{
    // <summary>
    /// Represents the action of delete record
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DeletionAction : Action
    {
        public DeletionAction() : base()
        {
            this.Type = ActionType.Delete;
        }

        [DataMember]
        public Condition Condition { get; set; }
    }
}
