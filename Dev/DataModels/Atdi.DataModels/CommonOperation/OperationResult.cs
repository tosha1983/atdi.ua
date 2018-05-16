using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.CommonOperation
{
    /// <summary>
    /// Represents the result of the operation without the returned data
    /// </summary>
    /// <typeparam name="TState">The type of the state of the operation</typeparam>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class OperationResult<TState>
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

    
}
