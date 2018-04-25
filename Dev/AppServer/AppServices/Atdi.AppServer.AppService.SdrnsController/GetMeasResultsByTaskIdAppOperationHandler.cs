using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.BusManager;


namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetMeasResultsByTaskIdAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetMeasResultsByTaskIdAppOperation,
            GetMeasResultsByTaskIdAppOperationOptions,
            MeasurementResults[]
        >
    {

        public GetMeasResultsByTaskIdAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }


        public override MeasurementResults[] Handle(GetMeasResultsByTaskIdAppOperationOptions options, IAppOperationContext operationContext)
        {
            Logger.Trace(this, options, operationContext);
            List<MeasurementResults> res = GlobalInit.LST_MeasurementResults.FindAll(t => t.Id.MeasTaskId.Value == options.TaskId.Value);
            if (res != null)
                return res.ToArray();
            else return null;
        }
    }

}