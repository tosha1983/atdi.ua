using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;



namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class CreateMeasTaskAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.CreateMeasTaskAppOperation,
            CreateMeasTaskAppOperationOptions,
            MeasTaskIdentifier
        >
    {
        public CreateMeasTaskAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override MeasTaskIdentifier Handle(CreateMeasTaskAppOperationOptions options, IAppOperationContext operationContext)
        {
            MeasTaskIdentifier md = new MeasTaskIdentifier();
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
            MeasTask mt = options.Task;
            if (mt.Id == null) mt.Id = new MeasTaskIdentifier();
            if (mt.Status == null) mt.Status = "N";
            WorkFlowProcessManageTasks tasks = new WorkFlowProcessManageTasks();
            System.Console.WriteLine("Start Create_New_Meas_Task ");
            int ID = tasks.Create_New_Meas_Task(mt, "New");
            md.Value = ID;
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread tsg = new System.Threading.Thread(() => {
            try {
                List<int> SensorIds = new List<int>();
                if (mt.Stations != null) {
                    foreach (MeasStation ts in mt.Stations) {
                        if (ts.StationId != null) {
                            if (ts.StationId!= null) {
                                if (!SensorIds.Contains(ts.StationId.Value))
                                    SensorIds.Add(ts.StationId.Value);
                            }
                        }
                    }
                }
                WorkFlowProcessManageTasks.Process_Multy_Meas(mt, SensorIds, "New", false);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("CreateMeasTaskAppOperationHandler "+ex.Message);
            }
            });
            tsg.Start();
            //tsg.Join();
            return md;
        }
    }
}
