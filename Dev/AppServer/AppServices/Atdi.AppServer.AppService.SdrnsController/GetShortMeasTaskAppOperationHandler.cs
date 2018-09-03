using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
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
                try
                {
                    List<MeasTask> ResMeasTasks = new List<MeasTask>();
                    //List<KeyValuePair<int, MeasTask>> mtsk = GlobalInit.blockingCollectionMeasTask.ToList().FindAll(t => t.Key == options.TaskId.Value);
                    //foreach (KeyValuePair<int, MeasTask> v in mtsk)
                    //ResMeasTasks.Add(v.Value);

                    ResMeasTasks = ts.ConvertToShortMeasTasks(cl.ShortReadTask(options.TaskId.Value)).ToList();
                    if (ResMeasTasks != null)
                    {
                        if (ResMeasTasks.Count > 0)
                        {
                            MeasTask mts = ResMeasTasks.ToList().Find(t => t.Status != "Z" && t.Id.Value == options.TaskId.Value);
                            if (mts != null)
                            {
                                var SMT = new ShortMeasTask { CreatedBy = mts.CreatedBy, DateCreated = mts.DateCreated, ExecutionMode = mts.ExecutionMode, Id = mts.Id, MaxTimeBs = mts.MaxTimeBs, Name = mts.Name, OrderId = mts.OrderId.GetValueOrDefault(), Prio = mts.Prio, ResultType = mts.ResultType, Status = mts.Status, Task = mts.Task, Type = mts.Type };
                                if (mts.MeasDtParam != null) SMT.TypeMeasurements = mts.MeasDtParam.TypeMeasurements;
                                Res = SMT;
                            }
                            else
                                Res = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            });
            th.Start();
            th.Join();
            return Res;
        }
    }

}