using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels
{
    /// <summary>
    /// Represents the action result
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class ActionResult
    {
        /// <summary>
        /// The ID of the action
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// The type of the action
        /// </summary>
        [DataMember]
        public ActionType Type { get; set; }
        
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

        /// <summary>
        /// The number of rows changed, inserted, or deleted by execution of the action
        /// </summary>
        [DataMember]
        public int RecordsAffected { get; set; }
    }
}
