using Atdi.DataModels.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.Identity
{
    public interface IAuthenticationManager
    {
        UserIdentity AuthenticateUser(UserCredential credential);

        ServiceIdentity AuthenticateService(ServiceCredential credential);

        AuthRedirectionQuery PrepareAuthRedirection(ServiceToken token, AuthRedirectionOptions options);

        UserIdentity HandleAndAuthenticateUser(ServiceToken token, AuthRedirectionResponse response);
	}
}
