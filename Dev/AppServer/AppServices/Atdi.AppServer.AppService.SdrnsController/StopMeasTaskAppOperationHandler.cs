using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using Atdi.SDNRS.AppServer.BusManager;


namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class StopMeasTaskAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.StopMeasTaskAppOperation,
            StopMeasTaskAppOperationOptions,
            CommonOperationResult
        >
    {

        public StopMeasTaskAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override CommonOperationResult Handle(StopMeasTaskAppOperationOptions options, IAppOperationContext operationContext)
        {
            CommonOperationResult res = new CommonOperationResult();
            System.Threading.Thread ge = new System.Threading.Thread(() =>
            {
                try
                {
                    if (options.TaskId != null)
                    {
                        MeasTask mt = GlobalInit.LIST_MEAS_TASK.Find(z => z.Id.Value == options.TaskId.Value);
                        if (mt != null)
                        {
                            WorkFlowProcessManageTasks tasks = new WorkFlowProcessManageTasks(Logger);
                            //int ID = tasks.Create_New_Meas_Task(mt, "Stop");
                            List<int> SensorIds = new List<int>();
                            foreach (MeasSubTask item in mt.MeasSubTasks)
                            {
                                foreach (MeasSubTaskStation u in item.MeasSubTaskStations)
                                {
                                    SensorIds.Add(u.StationId.Value);
                                }
                            }

                            foreach (MeasStation item in mt.Stations)
                            {
                                SensorIds.Add(item.StationId.Value);
                            }

                            var mt_edit = new MeasTask() { CreatedBy = mt.CreatedBy, DateCreated = mt.DateCreated, ExecutionMode = mt.ExecutionMode, Id = mt.Id, MaxTimeBs = mt.MaxTimeBs, MeasDtParam = mt.MeasDtParam, MeasFreqParam = mt.MeasFreqParam, MeasLocParams = mt.MeasLocParams, MeasOther = mt.MeasOther, MeasSubTasks = mt.MeasSubTasks, MeasTimeParamList = mt.MeasTimeParamList, Name = mt.Name, OrderId = mt.OrderId, Prio = mt.Prio, ResultType = mt.ResultType, Stations = mt.Stations, Status = mt.Status, Task = mt.Task, Type = mt.Type };
                            if (SensorIds.Count > 0)
                            {
                                bool isOnline = false;
                                tasks.Process_Multy_Meas(mt_edit, SensorIds, "Stop", isOnline);
                                res.State = CommonOperationState.Success;
                            }
                        }
                    }
                    
                    Logger.Trace(this, options, operationContext);
                }
                catch (Exception ex)
                {
                    Logger.Trace("Error in procedure StopMeasTaskAppOperationHandler: " + ex.Message);
                    res.State = CommonOperationState.Fault;
                    res.FaultCause = ex.Message;
                }
            });
            ge.Start();
            ge.IsBackground = true;
            ge.Join();
            return res;
        }
    }
}
