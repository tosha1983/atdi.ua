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
            List<ShortMeasTask> Res = new List<ShortMeasTask>();
            if (GlobalInit.LIST_MEAS_TASK != null) {
                if (GlobalInit.LIST_MEAS_TASK.Count() > 0) {
                    List<MeasTask> tsk = GlobalInit.LIST_MEAS_TASK.FindAll(t => t.Status != "Z");
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
            Logger.Trace(this, options, operationContext);
            return Res.ToArray();
        }
    }
}
