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
    public class GetMeasResultsByIdAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetMeasResultsByIdAppOperation,
            GetMeasResultsByIdAppOperationOptions,
            MeasurementResults
        >
    {

        public GetMeasResultsByIdAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }


        public override MeasurementResults Handle(GetMeasResultsByIdAppOperationOptions options, IAppOperationContext operationContext)
        {
            MeasurementResults res = new MeasurementResults();
            Logger.Trace(this, options, operationContext);
            if (options.MeasResultsId != null) {
                if (options.MeasResultsId.MeasTaskId != null) {
                    res = GlobalInit.LST_MeasurementResults.Find(t => t.Id.MeasSdrResultsId == options.MeasResultsId.MeasSdrResultsId && t.Id.MeasTaskId.Value == options.MeasResultsId.MeasTaskId.Value && t.Id.SubMeasTaskId == options.MeasResultsId.SubMeasTaskId && t.Id.SubMeasTaskStationId == options.MeasResultsId.SubMeasTaskStationId);
                }
            }
            return res;
        }
    }

}