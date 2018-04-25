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
    public class GetMeasTaskAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetMeasTaskAppOperation,
            GetMeasTaskAppOperationOptions,
            MeasTask
        >
    {
        public GetMeasTaskAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override MeasTask Handle(GetMeasTaskAppOperationOptions options, IAppOperationContext operationContext)
        {
            MeasTask Res = null;
            if (GlobalInit.LIST_MEAS_TASK != null) {
                if (GlobalInit.LIST_MEAS_TASK.Count() > 0) {
                    MeasTask tsk = GlobalInit.LIST_MEAS_TASK.Find(t => t.Status != "Z" && t.Id.Value== options.TaskId.Value);
                    if (tsk != null)
                        Res = tsk;
                    else
                        Res = null;
                }
            }
            Logger.Trace(this, options, operationContext);
            return Res;
        }
    }
}
