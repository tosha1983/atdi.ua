using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.DataModels.Sdrns.Server
{
    /// <summary>
    /// The common state of the operation
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public enum  CommonOperationState
    {
        /// <summary>
        /// The operation completed successfully
        /// </summary>
        [EnumMember]
        Success,

        /// <summary>
        /// The operation failed
        /// </summary>
        [EnumMember]
        Fault
    }
}
