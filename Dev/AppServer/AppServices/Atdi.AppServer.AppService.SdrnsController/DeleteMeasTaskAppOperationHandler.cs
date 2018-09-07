using System;
using System.Collections.Generic;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;



namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class DeleteMeasTaskAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.DeleteMeasTaskAppOperation,
            DeleteMeasTaskAppOperationOptions,
            CommonOperationResult
        >
    {

        public DeleteMeasTaskAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override CommonOperationResult Handle(DeleteMeasTaskAppOperationOptions options, IAppOperationContext operationContext)
        {
            CommonOperationResult res = new CommonOperationResult();
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    if (options.TaskId != null)
                    {
                        ClassesDBGetTasks cl = new ClassesDBGetTasks(Logger);
                        ClassesDBGetResult resDb = new ClassesDBGetResult(Logger);
                        ClassConvertTasks ts = new ClassConvertTasks(Logger);
                        ClassConvertToSDRResults conv = new ClassConvertToSDRResults(Logger);
                        MeasTask[] Res = ts.ConvertToShortMeasTasks(cl.ShortReadTask(options.TaskId.Value));
                        //MeasTask mt = GlobalInit.LIST_MEAS_TASK.Find(z => z.Id.Value == options.TaskId.Value);
                        MeasTask mt = null;
                        if (Res.Length > 0) mt = Res[0];
                        if (mt != null)
                        {
                            WorkFlowProcessManageTasks tasks = new WorkFlowProcessManageTasks(Logger);
                            List<int> SensorIds = new List<int>();
                            if (mt.MeasSubTasks != null)
                            {
                                foreach (MeasSubTask item in mt.MeasSubTasks)
                                {
                                    foreach (MeasSubTaskStation u in item.MeasSubTaskStations)
                                    {
                                        SensorIds.Add(u.StationId.Value);
                                    }
                                }
                            }

                            if (mt.Stations != null)
                            {
                                foreach (MeasStation item in mt.Stations)
                                {
                                    if (item.StationId.Value > 0)
                                    {
                                        SensorIds.Add(item.StationId.Value);
                                    }
                                }
                            }

                            var mt_edit = new MeasTask() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MaxTimeBs = mt.MaxTimeBs, MeasDtParam = mt.MeasDtParam, MeasFreqParam = mt.MeasFreqParam, MeasLocParams = mt.MeasLocParams, MeasOther = mt.MeasOther, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, OrderId = mt.OrderId, Prio = mt.Prio, ResultType = mt.ResultType, Stations = mt.Stations, Status = mt.Status, Task = mt.Task, Type = mt.Type };
                            bool isSuccessTemp = false;
                            if (SensorIds.Count > 0)
                            {
                                tasks.Process_Multy_Meas(mt_edit, SensorIds, "Stop", false, out isSuccessTemp);
                                res.State = isSuccessTemp == true ? CommonOperationState.Success : CommonOperationState.Fault;
                            }
                            else res.State = CommonOperationState.Fault;

                            if (tasks.Process_Multy_Meas(mt_edit, SensorIds, "Del", false, out isSuccessTemp))
                            res.State = isSuccessTemp == true ? CommonOperationState.Success : CommonOperationState.Fault;

                        }
                        else res.State = CommonOperationState.Fault;
                    }
                    else res.State = CommonOperationState.Fault;
                    Logger.Trace(this, options, operationContext);
                }
                catch (Exception ex)
                {
                    res.State = CommonOperationState.Fault;
                    res.FaultCause = ex.Message;
                    Logger.Error(ex.Message);
                }
            });
            th.Start();
            th.Join();
            return res;
        }
    }
}
