using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.WebQueryManager;
using Atdi.AppServer.Contracts.WebQuery;
using Atdi.AppServer.AppService.WebQueryDataDriver;

namespace Atdi.AppServer.AppServices.WebQueryManager
{
    public class AuthenticateUserAppOperationHandler
        : AppOperationHandlerBase
        <
            WebQueryManagerAppService,
            WebQueryManagerAppService.AuthenticateUserAppOperation,
            AuthenticateUserAppOperationOptions,
            int
        >
    {
        public AuthenticateUserAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
            
        }

        /// <summary>
        /// Authentication of the current user by user name and password
        /// </summary>
        /// <param name="options">The options of this operation</param>
        /// <param name="operationContext">The context of this operation</param>
        /// <returns>Return user id or 0 when user has incorrect a user name or password</returns>
        public override int Handle(AuthenticateUserAppOperationOptions options, IAppOperationContext operationContext)
        {
            int Id_User = -1;
            try { 
            ConnectDB conn = new ConnectDB();
            Id_User = conn.AuthorizeRetID(options.UserName, options.Password);
            Logger.Trace(this, options, operationContext);
            }
            catch (Exception ex) { Logger.Error(ex); }
            return Id_User;
        }
    }
}
