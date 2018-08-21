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
            ClassesDBGetResult resDb = new ClassesDBGetResult(Logger);
            ClassConvertToSDRResults conv = new ClassConvertToSDRResults(Logger);
            List<MeasurementResults> LST_MeasurementResults = conv.ConvertTo_SDRObjects(resDb.ReadResultFromDBTask(options.MeasResultsId.MeasTaskId.Value)).ToList();
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                if (options.MeasResultsId != null)
                {
                    if (options.MeasResultsId.MeasTaskId != null)
                    {
                        res = LST_MeasurementResults.Find(t => t.Id.MeasSdrResultsId == options.MeasResultsId.MeasSdrResultsId && t.Id.MeasTaskId.Value == options.MeasResultsId.MeasTaskId.Value && t.Id.SubMeasTaskId == options.MeasResultsId.SubMeasTaskId && t.Id.SubMeasTaskStationId == options.MeasResultsId.SubMeasTaskStationId);
                    }
                }
            });
            th.Start();
            th.IsBackground = true;
            th.Join();
            return res;
        }
    }

}