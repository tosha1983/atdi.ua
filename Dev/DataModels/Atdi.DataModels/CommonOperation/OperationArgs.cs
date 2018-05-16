using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.CommonOperation
{
    /// <summary>
    /// Represents the dictionary of other arguments of the operation
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class OperationArgs
    {
        /// <summary>
        /// The dictionary of the values of some agreed operation arguments 
        /// </summary>
        [DataMember]
        public Dictionary<string, object> Values { get; set; }
    }

    
}
