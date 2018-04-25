using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts
{
    /// <summary>
    /// Represents the result of the operation without the returned data
    /// </summary>
    /// <typeparam name="TState">The type of the state of the operation</typeparam>
    [DataContract(Namespace = CommonServicesSpecification.Namespace)]
    public class CommonOperationResult<TState>
    {
        /// <summary>
        /// The state of the operation
        /// </summary>
        [DataMember]
        public TState State { get; set; }

        /// <summary>
        /// The message describing the fault of the operation
        /// </summary>
        [DataMember]
        public string FaultCause { get; set; }
    }

    /// <summary>
    /// Represents the result of the operation with the common state:  Success or Fault
    /// </summary>
    [DataContract(Namespace = CommonServicesSpecification.Namespace)]
    public class CommonOperationResult : CommonOperationResult<CommonOperationState>
    {
    }
}
