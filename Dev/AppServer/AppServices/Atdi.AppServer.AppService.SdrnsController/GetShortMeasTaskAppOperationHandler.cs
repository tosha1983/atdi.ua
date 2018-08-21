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
            ClassesDBGetTasks cl = new ClassesDBGetTasks(Logger);
            ClassConvertTasks ts = new ClassConvertTasks(Logger);
            ShortMeasTask Res = new ShortMeasTask();
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                MeasTask[] ResMeasTasks = ts.ConvertTo_MEAS_TASKObjects(cl.ReadTask(options.TaskId.Value));
                if (ResMeasTasks != null) {
                if (ResMeasTasks.Length > 0) {
                    MeasTask mts = ResMeasTasks.ToList().Find(t => t.Status != "Z" && t.Id.Value == options.TaskId.Value);
                    if (mts != null) {
                            var SMT = new ShortMeasTask { CreatedBy = mts.CreatedBy, DateCreated = mts.DateCreated, ExecutionMode = mts.ExecutionMode, Id = mts.Id, MaxTimeBs = mts.MaxTimeBs, Name = mts.Name, OrderId = mts.OrderId.GetValueOrDefault(), Prio = mts.Prio, ResultType = mts.ResultType, Status = mts.Status, Task = mts.Task, Type = mts.Type };
                            if (mts.MeasDtParam != null) SMT.TypeMeasurements = mts.MeasDtParam.TypeMeasurements;
                            Res = SMT;
                    }
                    else
                        Res = null;
                }
            }
            });
            th.Start();
            th.IsBackground = true;
            th.Join();
            return Res;
        }
    }

}