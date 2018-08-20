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
using Atdi.SDNRS.AppServer.ManageDB.Adapters;

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetMeasResultsAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetMeasResultsAppOperation,
            GetMeasResultsAppOperationOptions,
            MeasurementResults[]
        >
    {
        public GetMeasResultsAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override MeasurementResults[] Handle(GetMeasResultsAppOperationOptions options, IAppOperationContext operationContext)
        {
            Logger.Trace(this, options, operationContext);
            ClassesDBGetResult resDb = new ClassesDBGetResult(Logger);
            ClassConvertToSDRResults conv = new ClassConvertToSDRResults(Logger);
            MeasurementResults[] LST_MeasurementResults = conv.ConvertTo_SDRObjects(resDb.ReadlAllResultFromDB());
            return LST_MeasurementResults;
        }
    }
}
