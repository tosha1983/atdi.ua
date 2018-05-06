using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.WebQueryManager;
using Atdi.AppServer.Contracts.WebQuery;
using Atdi.AppServer.AppService.WebQueryDataDriver.ICSMUtilities;


namespace Atdi.AppServer.AppServices.WebQueryManager
{
    public class GetQueryTreeAppOperationHandler
        : AppOperationHandlerBase
        <
            WebQueryManagerAppService,
            WebQueryManagerAppService.GetQueryTreeAppOperation,
            GetQueryTreeAppOperationOptions,
            QueryTree
        >
    {
        public GetQueryTreeAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        public override QueryTree Handle(GetQueryTreeAppOperationOptions options, IAppOperationContext operationContext)
        {
            QueryTree QTree = new QueryTree();
            try {
                UtilsDef Menu = new UtilsDef(ref QTree, options.OtherArgs.UserId);
                Logger.Trace(this, options, operationContext);
            }
            catch (Exception ex) { Logger.Error(ex); }
            return QTree;
        }
    }




}
