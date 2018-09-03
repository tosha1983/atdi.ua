using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.CommonOperation
{
    /// <summary>
    /// The common state of the operation
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public enum  OperationState
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
