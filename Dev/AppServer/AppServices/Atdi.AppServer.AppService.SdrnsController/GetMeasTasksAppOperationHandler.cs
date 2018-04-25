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
    public class GetMeasTasksAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetMeasTasksAppOperation,
            GetMeasTasksAppOperationOptions,
            MeasTask[]
        >
    {
        public GetMeasTasksAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override MeasTask[] Handle(GetMeasTasksAppOperationOptions options, IAppOperationContext operationContext)
        {
            MeasTask[] Res = null;
            if (GlobalInit.LIST_MEAS_TASK != null) {
                if (GlobalInit.LIST_MEAS_TASK.Count() > 0) {
                    List<MeasTask> tsk = GlobalInit.LIST_MEAS_TASK.FindAll(t => t.Status != "Z");
                    if (tsk != null)
                        Res = tsk.ToArray();
                    else
                        Res = null;
                    if (tsk != null) {
                        if (tsk.Count == 0)   Res = null;
                    }
                }
            }
            Logger.Trace(this, options, operationContext);
            return Res;
        }
    }
}
