using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts
{
    /// <summary>
    /// Represents the dictionary of other arguments of the operation
    /// </summary>
    [DataContract(Namespace = CommonServicesSpecification.Namespace)]
    public class CommonOperationArguments
    {
        /// <summary>
        /// The Id of the current user
        /// </summary>
        [DataMember]
        public int UserId { get; set; }

        /// <summary>
        /// The dictionary of the values of some agreed operation arguments 
        /// </summary>
        [DataMember]
        public Dictionary<string, object> Values { get; set; }
    }
}
