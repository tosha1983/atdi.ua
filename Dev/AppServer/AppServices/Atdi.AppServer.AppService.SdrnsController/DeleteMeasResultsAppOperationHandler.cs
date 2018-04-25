using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;


namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class DeleteMeasResultsAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.DeleteMeasResultsAppOperation,
            DeleteMeasResultsAppOperationOptions,
            CommonOperationDataResult<int>
        >
    {
        public DeleteMeasResultsAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override CommonOperationDataResult<int> Handle(DeleteMeasResultsAppOperationOptions options, IAppOperationContext operationContext)
        {
            Logger.Trace(this, options, operationContext);

            return new CommonOperationDataResult<int>() { State = CommonOperationState.Success };
        }
    }
}
