﻿using System;
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
    public class GetShortMeasTasksAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetShortMeasTasksAppOperation,
            GetShortMeasTasksAppOperationOptions,
            ShortMeasTask[]
        >
    {
        public GetShortMeasTasksAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override ShortMeasTask[] Handle(GetShortMeasTasksAppOperationOptions options, IAppOperationContext operationContext)
        {
            ClassesDBGetTasks cl = new ClassesDBGetTasks(Logger);
            ClassConvertTasks ts = new ClassConvertTasks(Logger);
            MeasTask[] ResMeasTasks = ts.ConvertTo_MEAS_TASKObjects(cl.ReadlAllSTasksFromDB());
            List<ShortMeasTask> Res = new List<ShortMeasTask>();
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                if (ResMeasTasks != null) {
                if (ResMeasTasks.Count() > 0) {
                    List<MeasTask> tsk = ResMeasTasks.ToList().FindAll(t => t.Status != "Z");
                    if (tsk != null) {
                        foreach (MeasTask mts in tsk) {
                            var SMT = new ShortMeasTask { CreatedBy= mts.CreatedBy, DateCreated = mts.DateCreated, ExecutionMode = mts.ExecutionMode, Id = mts.Id, MaxTimeBs = mts.MaxTimeBs, Name = mts.Name, OrderId = mts.OrderId.GetValueOrDefault(), Prio = mts.Prio, ResultType = mts.ResultType, Status = mts.Status, Task = mts.Task, Type = mts.Type };
                            if (mts.MeasDtParam != null) SMT.TypeMeasurements = mts.MeasDtParam.TypeMeasurements;
                            Res.Add(SMT);
                        }
                    }
                    else
                        Res = null;
                    if (tsk != null)
                    {
                        if (tsk.Count == 0) Res = null;
                    }
                }
            }
            });
            th.Start();
            th.IsBackground = true;
            th.Join();
            Logger.Trace(this, options, operationContext);
            return Res.ToArray();
        }
    }
}
