
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.WcfServices.Identity;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.Identity;
using Atdi.Platform.Logging;

namespace Atdi.WcfServices.Identity
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AuthenticationManager : WcfServiceBase<IAuthenticationManager>, IAuthenticationManager
    {
        private readonly ILogger _logger;

        public AuthenticationManager(ILogger logger)
        {
            this._logger = logger;
        }

        public Result<UserIdentity> AuthenticateUser(UserCredential credential)
        {
            using (this._logger.StartTrace(Contexts.AuthenticationManager, Categories.OperationCall, TraceScopeNames.AuthenticationUser))
            {
                return new Result<UserIdentity>
                {
                    State = OperationState.Success
                };
            }
        }
    }
}
