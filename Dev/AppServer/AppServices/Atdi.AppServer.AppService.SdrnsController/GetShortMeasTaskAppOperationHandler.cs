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
    public class GetShortMeasTaskAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetShortMeasTaskAppOperation,
            GetShortMeasTaskAppOperationOptions,
            ShortMeasTask
        >
    {

        public GetShortMeasTaskAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }


        public override ShortMeasTask Handle(GetShortMeasTaskAppOperationOptions options, IAppOperationContext operationContext)
        {
            ShortMeasTask Res = new ShortMeasTask();
            if (GlobalInit.LIST_MEAS_TASK != null) {
                if (GlobalInit.LIST_MEAS_TASK.Count() > 0) {
                    MeasTask mts = GlobalInit.LIST_MEAS_TASK.Find(t => t.Status != "Z" && t.Id.Value == options.TaskId.Value);
                    if (mts != null) {
                            var SMT = new ShortMeasTask { CreatedBy = mts.CreatedBy, DateCreated = mts.DateCreated, ExecutionMode = mts.ExecutionMode, Id = mts.Id, MaxTimeBs = mts.MaxTimeBs, Name = mts.Name, OrderId = mts.OrderId.GetValueOrDefault(), Prio = mts.Prio, ResultType = mts.ResultType, Status = mts.Status, Task = mts.Task, Type = mts.Type };
                            if (mts.MeasDtParam != null) SMT.TypeMeasurements = mts.MeasDtParam.TypeMeasurements;
                            Res = SMT;
                    }
                    else
                        Res = null;
                }
            }
            return Res;
        }
    }

}