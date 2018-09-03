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
                var userIdentity = this._coreAuthManager.AuthenticateUser(options.Credential);
                return userIdentity;
            }
        }
    }
}
