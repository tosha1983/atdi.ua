using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetRoutesAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetRoutesAppOperation,
            GetShortMeasResStationAppOperationOptions,
            Route[]
        >
    {

        public GetRoutesAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }


        public override Route[] Handle(GetShortMeasResStationAppOperationOptions options, IAppOperationContext operationContext)
        {
            Route[] routes = null;
            ClassesDBGetResult resDb = new ClassesDBGetResult(Logger);
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    if (options.ResId != null)
                    {
                        routes =resDb.ReadRoutes(options.ResId.Value);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            });
            th.Start();
            th.Join();
            return routes.ToArray();
        }
    }

}