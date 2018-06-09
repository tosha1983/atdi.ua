using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.Identity
{
    /// <summary>
    /// Represents the simple user token
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class UserToken
    {
        /// <summary>
        /// The Id of the user
        /// </summary>
        [DataMember]
        public byte[] Data { get; set; }
    }
}
