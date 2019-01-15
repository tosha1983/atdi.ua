using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;


namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetStationDataForMeasurementsByTaskIdAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetStationDataForMeasurementsByTaskIdAppOperation,
            GetMeasTaskAppOperationOptions,
            StationDataForMeasurements[]
        >
    {
        public GetStationDataForMeasurementsByTaskIdAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override StationDataForMeasurements[] Handle(GetMeasTaskAppOperationOptions options, IAppOperationContext operationContext)
        {
            StationDataForMeasurements[] val = null;
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                try
                { 
                ClassesDBGetTasksOptimize cl = new ClassesDBGetTasksOptimize(Logger);
                 ClassConvertTasksOptimize ts = new ClassConvertTasksOptimize(Logger);
                List<MeasTask> getValues = new List<MeasTask>();
                List<MeasTask> Res = new List<MeasTask>();
                 val = ts.ConvertToStationDataForMeasurements(cl.ReadTaskHeader(options.TaskId.Value));
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
