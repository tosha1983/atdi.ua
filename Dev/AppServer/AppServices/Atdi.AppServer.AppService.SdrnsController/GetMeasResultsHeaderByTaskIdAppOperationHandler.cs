using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;


namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetMeasResultsHeaderByTaskIdAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetMeasResultsHeaderByTaskIdAppOperation,
            GetMeasResultsByTaskIdAppOperationOptions,
            MeasurementResults[]
        >
    {

        public GetMeasResultsHeaderByTaskIdAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }


        public override MeasurementResults[] Handle(GetMeasResultsByTaskIdAppOperationOptions options, IAppOperationContext operationContext)
        {
            List<MeasurementResults> res = new List<MeasurementResults>();
            ClassesDBGetResultOptimize resDb = new ClassesDBGetResultOptimize(Logger);
            ClassConvertToSDRResultsOptimize conv = new ClassConvertToSDRResultsOptimize(Logger);
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    List<MeasurementResults> LST_MeasurementResults = conv.ConvertHeader(resDb.ReadResultHeaderFromTaskId(options.TaskId.Value)).ToList();
                    res = LST_MeasurementResults.FindAll(t => t.Id.MeasTaskId.Value == options.TaskId.Value);

                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                }
            });
            th.Start();
            th.Join();
            return res.ToArray();
        }
    }

}

