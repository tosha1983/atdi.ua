
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.WcfServices.Identity;
using CI = Atdi.Contracts.CoreServices.Identity;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.Identity;
using Atdi.Platform.Logging;

namespace Atdi.WcfServices.Identity
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AuthenticationManager : WcfServiceBase<IAuthenticationManager>, IAuthenticationManager
    {
        private readonly ILogger _logger;
        private readonly CI.IAuthenticationManager _coreAuthManager;

        public AuthenticationManager(CI.IAuthenticationManager coreAuthManager, ILogger logger)
        {
            this._logger = logger;
            this._coreAuthManager = coreAuthManager;
        }

		public Result<ServiceIdentity> AuthenticateService(ServiceCredential credential)
		{
			throw new NotImplementedException();
		}

		public Result<UserIdentity> AuthenticateUser(UserCredential credential)
        {
            using (this._logger.StartTrace(Contexts.AuthenticationManager, Categories.OperationCall, TraceScopeNames.AuthenticationUser))
            {
                var userToken = this._coreAuthManager.AuthenticateUser(credential);
                return new Result<UserIdentity>
                {
                    State = OperationState.Success,
                    Data = userToken,
                };
            }
        }

		public Result<UserIdentity> HandleAndAuthenticateUser(ServiceToken token, AuthRedirectionResponse response)
		{
			throw new NotImplementedException();
		}

		public Result<AuthRedirectionQuery> PrepareAuthRedirection(ServiceToken token, AuthRedirectionOptions options)
		{
			throw new NotImplementedException();
		}
	}
}
