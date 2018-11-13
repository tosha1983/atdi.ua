using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;


namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetMeasTaskAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetMeasTaskAppOperation,
            GetMeasTaskAppOperationOptions,
            MeasTask
        >
    {
        public GetMeasTaskAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override MeasTask Handle(GetMeasTaskAppOperationOptions options, IAppOperationContext operationContext)
        {
            /*
            MeasTask val = null;
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                try
                { 
                ClassesDBGetTasks cl = new ClassesDBGetTasks(Logger);
                ClassConvertTasks ts = new ClassConvertTasks(Logger);
                List<MeasTask> getValues = new List<MeasTask>();
                List<MeasTask> Res = new List<MeasTask>();
                Res = ts.ConvertTo_MEAS_TASKObjects(cl.ReadTask(options.TaskId.Value)).ToList();
                if (Res != null)
                    {
                        if (Res.Count() > 0)
                        {
                            List<MeasTask> tsk = Res.ToList().FindAll(t => t.Status != "Z");
                            if (tsk != null)
                                val = tsk[0];
                            else
                                val = null;
                            if (tsk != null)
                            {
                                if (tsk.Count == 0) val = null;
                            }
                        }
                    }
                    Logger.Trace(this, options, operationContext);
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message);
                }
            });
            thread.Start();
            thread.Join();
            return val;
            */
            throw new NotImplementedException("Method GetMeasTask not implemented.");
        }
    }
}
