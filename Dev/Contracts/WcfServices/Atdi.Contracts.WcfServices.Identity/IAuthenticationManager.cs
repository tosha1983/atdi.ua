using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.Identity;

namespace Atdi.Contracts.WcfServices.Identity
{
    /// <summary>
    /// The public contract of the WCF-service of the authentication manager
    /// </summary>
    [ServiceContract(Namespace = Specification.Namespace)]
    public interface IAuthenticationManager
    {
        /// <summary>
        /// Authentication of the current user by user name and password
        /// </summary>
        /// <param name="credential">The credential of the user</param>
        /// <returns>The user identity</returns>
        [OperationContract]
        Result<UserIdentity> AuthenticateUser(UserCredential credential);
    }
}
