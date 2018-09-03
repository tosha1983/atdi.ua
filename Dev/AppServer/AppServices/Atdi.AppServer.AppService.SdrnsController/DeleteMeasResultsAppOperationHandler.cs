using System;
using System.Collections.Generic;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;


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
            CommonOperationDataResult<int> cv_r = new CommonOperationDataResult<int>();
            List<MeasurementResults> LST_MeasurementResults = new List<MeasurementResults>();
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    ClassesDBGetResult resDb = new ClassesDBGetResult(Logger);
                    if (options.MeasResultsId != null)
                    {
                        if (resDb.DeleteResultFromDB(options.MeasResultsId, "Z"))
                        {
                            cv_r.State = CommonOperationState.Success;
                        }
                        else cv_r.State = CommonOperationState.Fault;
                    }
                    else cv_r.State = CommonOperationState.Fault;
                }
                catch (Exception ex)
                {
                    cv_r.State = CommonOperationState.Fault;
                    cv_r.FaultCause = ex.Message;
                    Logger.Error(ex.Message);
                }
            });
            th.Start();
            th.Join();
            Logger.Trace(this, options, operationContext);
            return cv_r;
        }
    }
}
