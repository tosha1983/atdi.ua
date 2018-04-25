using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts
{
    /// <summary>
    /// Represents the result of the operation with the returned data of type TReturnedData and with the state of type TState
    /// </summary>
    /// <typeparam name="TState">The type of the state of the operation</typeparam>
    /// <typeparam name="TReturnedData">The type of the returned data of the operation</typeparam>
    [DataContract(Namespace = CommonServicesSpecification.Namespace)]
    public class CommonOperationDataResult<TState, TReturnedData> : CommonOperationResult<TState>
    {
        /// <summary>
        /// The returned data of the operation
        /// </summary>
        [DataMember]
        public TReturnedData Data { get; set; }
    }

    /// <summary>
    /// Represents the result of the operation with the returned data of type TReturnedData and with the common state:  Success or Fault
    /// </summary>
    /// <typeparam name="TReturnedData">The type of the returned data of the operation</typeparam>
    [DataContract(Namespace = CommonServicesSpecification.Namespace)]
    public class CommonOperationDataResult<TReturnedData> : CommonOperationResult
    {
        /// <summary>
        /// The returned data of the operation
        /// </summary>
        [DataMember]
        public TReturnedData Data { get; set; }
    }
}
