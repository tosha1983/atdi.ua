using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.Identity
{
    /// <summary>
    /// Represents the dictionary of other arguments of the operation
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class UserIdentity<TUserId, TUserToken>
    {
        /// <summary>
        /// The id of user
        /// </summary>
        [DataMember]
        public TUserId Id { get; set; }

        /// <summary>
        /// The name of user
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The token of current user
        /// </summary>
        [DataMember]
        public TUserToken UserToken { get; set; }

    }

    /// <summary>
    /// Represents the dictionary of other arguments of the operation with the simple user token
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class UserIdentity : UserIdentity<int, UserToken>
    {
    }
}
