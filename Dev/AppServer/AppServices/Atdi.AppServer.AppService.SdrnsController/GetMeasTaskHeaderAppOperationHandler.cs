using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;


namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetMeasTaskHeaderAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetMeasTaskHeaderAppOperation,
            GetMeasTaskAppOperationOptions,
            MeasTask
        >
    {
        public GetMeasTaskHeaderAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
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
            MeasTask val = null;
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                try
                { 
                ClassesDBGetTasksOptimize cl = new ClassesDBGetTasksOptimize(Logger);
                 ClassConvertTasksOptimize ts = new ClassConvertTasksOptimize(Logger);
                List<MeasTask> getValues = new List<MeasTask>();
                List<MeasTask> Res = new List<MeasTask>();
                Res = ts.ConvertToMeastTaskHeader(cl.ReadTaskHeader(options.TaskId.Value)).ToList();
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
        }
    }
}
