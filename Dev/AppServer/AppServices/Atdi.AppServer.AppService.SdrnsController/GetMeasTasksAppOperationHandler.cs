using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetMeasTasksAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetMeasTasksAppOperation,
            GetMeasTasksAppOperationOptions,
            MeasTask[]
        >
    {
        public GetMeasTasksAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override MeasTask[] Handle(GetMeasTasksAppOperationOptions options, IAppOperationContext operationContext)
        {
            /*
            MeasTask[] Res = null;
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                try
                {
                    ClassesDBGetTasks cl = new ClassesDBGetTasks(Logger);
                    ClassConvertTasks ts = new ClassConvertTasks(Logger);
                    List<MeasTask> resEnumerable = new List<MeasTask>();
                    Res = ts.ConvertTo_MEAS_TASKObjects(cl.ReadlAllSTasksFromDB());
                    if (Res != null)
                    {
                        if (Res.Count() > 0)
                        {
                            List<MeasTask> tsk = Res.ToList().FindAll(t => t.Status != "Z");
                            if (tsk != null)
                                Res = tsk.ToArray();
                            else
                                Res = null;
                            if (tsk != null)
                            {
                                if (tsk.Count == 0) Res = null;
                            }
                        }
                    }
                    Logger.Trace(this, options, operationContext);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            });
            thread.Start();
            thread.Join();
            return Res;
            */
            throw new NotImplementedException("Method GetMeasTasks not implemented.");
        }
    }
}
