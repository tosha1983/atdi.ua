using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
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
            //List<MeasurementResults> LST_MeasurementResults = GlobalInit.blockingCollectionMeasurementResults.ToList().FindAll(t=>t.Id.MeasSdrResultsId== options.MeasResultsId.MeasSdrResultsId);
            List<MeasurementResults> LST_MeasurementResults = new List<MeasurementResults>();
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    if (options.MeasResultsId != null)
                    {
                        LST_MeasurementResults = conv.ConvertTo_SDRObjects(resDb.ReadResultFromDB(options.MeasResultsId)).ToList();
                        if (LST_MeasurementResults != null)
                        {
                            if (LST_MeasurementResults.Count > 0) res = LST_MeasurementResults[0];
                        }
                    }
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                }
            });
            th.Start();
            th.Join();
            return res;
        }
    }

}