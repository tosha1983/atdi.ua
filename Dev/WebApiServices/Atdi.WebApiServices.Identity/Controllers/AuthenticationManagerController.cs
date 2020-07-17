using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CI = Atdi.Contracts.CoreServices.Identity;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.Identity;
using Atdi.Platform.Logging;
using System.Web.Http;

namespace Atdi.WebApiServices.Identity.Controllers
{
    public class AuthenticationManagerController : WebApiController
    {
        public class AuthenticateUserOptions
        {
            public UserCredential Credential { get; set; }
        }

        public class AuthenticateServiceOptions
        {
	        public ServiceCredential Credential { get; set; }
        }

        public class PrepareAuthRedirectionOptions
        {
	        public ServiceToken Token { get; set; }

			public AuthRedirectionOptions Options{ get; set; }
		}
        public class HandleAndAuthenticateUserOptions
		{
	        public ServiceToken Token { get; set; }

	        public AuthRedirectionResponse Response { get; set; }
        }

		private readonly CI.IAuthenticationManager _coreAuthManager;

        public AuthenticationManagerController(CI.IAuthenticationManager coreAuthManager, ILogger logger) : base(logger)
        {
            this._coreAuthManager = coreAuthManager;
        }

        [HttpPost]
        public UserIdentity AuthenticateUser(AuthenticateUserOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.Credential == null)
            {
                throw new ArgumentNullException(nameof(options.Credential));
            }
            using (this.Logger.StartTrace(Contexts.AuthenticationManager, Categories.OperationCall, TraceScopeNames.AuthenticationUser))
            {
                var identity = this._coreAuthManager.AuthenticateUser(options.Credential);
                return identity;
            }
        }

        [HttpPost]
		public ServiceIdentity AuthenticateService(AuthenticateServiceOptions options)
        {
	        if (options == null)
	        {
		        throw new ArgumentNullException(nameof(options));
	        }
	        if (options.Credential == null)
	        {
		        throw new ArgumentNullException(nameof(options.Credential));
	        }
	        using (this.Logger.StartTrace(Contexts.AuthenticationManager, Categories.OperationCall, TraceScopeNames.AuthenticationService))
	        {
		        var userIdentity = this._coreAuthManager.AuthenticateService(options.Credential);
		        return userIdentity;
	        }
		}

		[HttpPost]
		public AuthRedirectionQuery PrepareAuthRedirection(PrepareAuthRedirectionOptions options)
		{
			if (options == null)
			{
				throw new ArgumentNullException(nameof(options));
			}
			if (options.Token == null)
			{
				throw new ArgumentNullException(nameof(options.Token));
			}
			if (options.Options == null)
			{
				throw new ArgumentNullException(nameof(options.Options));
			}
			using (this.Logger.StartTrace(Contexts.AuthenticationManager, Categories.OperationCall, TraceScopeNames.PrepareAuthRedirection))
			{
				var query = this._coreAuthManager.PrepareAuthRedirection(options.Token, options.Options);
				return query;
			}
		}

		[HttpPost]
		public UserIdentity HandleAndAuthenticateUser(HandleAndAuthenticateUserOptions options)
		{
			if (options == null)
			{
				throw new ArgumentNullException(nameof(options));
			}
			if (options.Token == null)
			{
				throw new ArgumentNullException(nameof(options.Token));
			}
			if (options.Response == null)
			{
				throw new ArgumentNullException(nameof(options.Response));
			}
			using (this.Logger.StartTrace(Contexts.AuthenticationManager, Categories.OperationCall, TraceScopeNames.HandleAndAuthenticateUser))
			{
				var identity = this._coreAuthManager.HandleAndAuthenticateUser(options.Token, options.Response);
				return identity;
			}
		}
	}
}
