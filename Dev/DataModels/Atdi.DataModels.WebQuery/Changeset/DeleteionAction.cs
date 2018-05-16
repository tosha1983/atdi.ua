using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.WebQuery
{
    // <summary>
    /// Represents the action of delete record
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class DeleteionAction : Action
    {
        [DataMember]
        public int RecordId { get; set; }
    }
}
