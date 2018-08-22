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
            List<MeasurementResults> res = new List<MeasurementResults>();
            ClassesDBGetResult resDb = new ClassesDBGetResult(Logger);
            ClassConvertToSDRResults conv = new ClassConvertToSDRResults(Logger);
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                List<MeasurementResults> LST_MeasurementResults = conv.ConvertTo_SDRObjects(resDb.ReadResultFromDBTask(options.TaskId.Value)).ToList();
                res = LST_MeasurementResults.FindAll(t => t.Id.MeasTaskId.Value == options.TaskId.Value);
            });
            th.Start();
            th.IsBackground = true;
            th.Join();
            return res.ToArray();
        }
    }

}