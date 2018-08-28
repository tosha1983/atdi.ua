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
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    MeasTask mt = options.Task;
                    if (mt.Id == null) mt.Id = new MeasTaskIdentifier();
                    if (mt.Status == null) mt.Status = "N";
                    WorkFlowProcessManageTasks tasks = new WorkFlowProcessManageTasks(Logger);
                    Logger.Trace("Start Create_New_Meas_Task... ");
                    int ID = tasks.Create_New_Meas_Task(mt, "New");
                    md.Value = ID;
                    Logger.Trace(this, options, operationContext);
                    List<int> SensorIds = new List<int>();
                    if (mt.Stations != null)
                    {
                        foreach (MeasStation ts in mt.Stations)
                        {
                            if (ts.StationId != null)
                            {
                                if (ts.StationId != null)
                                {
                                    if (!SensorIds.Contains(ts.StationId.Value))
                                        SensorIds.Add(ts.StationId.Value);
                                }
                            }
                        }
                    }
                    if (SensorIds.Count > 0)
                    {
                        tasks.Process_Multy_Meas(mt, SensorIds, "New", false);
                    }
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                }
            });
            th.Start();
            th.Join();
            return md;
        }
    }
}
