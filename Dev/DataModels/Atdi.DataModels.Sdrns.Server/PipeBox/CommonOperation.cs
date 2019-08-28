using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.DataModels.Sdrns.Server
{
    
    [DataContract(Namespace =Specification.Namespace)]
    [Serializable]
    public class CommonOperation<TState>
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
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class CommonOperation : CommonOperation<CommonOperationState>
    {
    }
}
