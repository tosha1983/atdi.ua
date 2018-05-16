using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.WebQuery
{
    /// <summary>
    /// Represents the action result
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class ActionResult
    {
        /// <summary>
        /// The type of the action
        /// </summary>
        [DataMember]
        public ActionType Type { get; set; }

        /// <summary>
        /// The id of the record
        /// </summary>
        [DataMember]
        public int? RecordId { get; set; }

        /// <summary>
        /// The status of the result
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// The message of the result
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }
}
