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

        /// <summary>
        /// Authentication of the external service by sid and secret key
        /// </summary>
        /// <param name="credential">The credential of the user</param>
        /// <returns>The user identity</returns>
        [OperationContract]
        Result<ServiceIdentity> AuthenticateService(ServiceCredential credential);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        [OperationContract]
        Result<AuthRedirectionQuery> PrepareAuthRedirection(ServiceToken token, AuthRedirectionOptions options);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="token"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		[OperationContract]
		Result<UserIdentity> HandleAndAuthenticateUser(ServiceToken token, AuthRedirectionResponse response);
	}
}
