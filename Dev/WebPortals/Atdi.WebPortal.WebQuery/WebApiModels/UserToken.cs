using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// Represents the simple user token
    /// </summary>
    [DataContract]
    public class UserToken
    {
        /// <summary>
        /// The Id of the user
        /// </summary>
        [DataMember]
        public byte[] Data { get; set; }
    }
}
