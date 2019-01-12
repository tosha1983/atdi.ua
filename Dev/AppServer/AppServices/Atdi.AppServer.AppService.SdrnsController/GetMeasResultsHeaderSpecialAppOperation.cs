using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;


namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetMeasResultsHeaderSpecialAppOperation
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetMeasResultsHeaderSpecialAppOperation,
            GetShortMeasResultsSpecialAppOperationOptions,
            MeasurementResults[]
        >
    {
        public GetMeasResultsHeaderSpecialAppOperation(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override MeasurementResults[] Handle(GetShortMeasResultsSpecialAppOperationOptions options, IAppOperationContext operationContext)
        {
            MeasurementResults[] res = null;
            ClassesDBGetResultOptimize resDb = new ClassesDBGetResultOptimize(Logger);
            ClassConvertToSDRResultsOptimize conv = new ClassConvertToSDRResultsOptimize(Logger);
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    res = conv.ConvertMeasurementResults(resDb.ReadlAllResultFromDB(options.measurementType)).ToArray();
                   
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            });
            th.Start();
            th.Join();
            return res;
        }
    }
}
