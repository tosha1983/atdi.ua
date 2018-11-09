using System;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
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
            MeasurementResults[] LST_MeasurementResults = null;
            ClassesDBGetResult resDb = new ClassesDBGetResult(Logger);
            ClassConvertToSDRResults conv = new ClassConvertToSDRResults(Logger);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    LST_MeasurementResults = conv.ConvertTo_SDRObjects(resDb.ReadlAllResultFromDB()).ToArray();
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                }
        });
            th.Start();
            th.Join();
            return LST_MeasurementResults;
        }
    }
}
