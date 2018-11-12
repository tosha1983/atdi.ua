using System;
using System.Collections.Generic;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.BusManager;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;

//using Atdi.AppServer.AppService.SdrnsController.ConstraintParsers;

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetStationLevelsByTaskAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetStationLevelsByTaskIdAppOperation,
            GetStationLevelsByTaskIdAppOperationOptions,
            StationLevelsByTask[]
        >
    {

        public GetStationLevelsByTaskAppOperationHandler(IAppServerContext serverContext, ILogger logger)
            : base(serverContext, logger)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override StationLevelsByTask[] Handle(GetStationLevelsByTaskIdAppOperationOptions options, IAppOperationContext operationContext)
        {
            Logger.Trace(this, options, operationContext);
            CalcStationLevelsByTask analiticsCalcStationLevelsByTask = new CalcStationLevelsByTask(Logger);
            StationLevelsByTask[] LstS = null;
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    LstS = analiticsCalcStationLevelsByTask.GetStationLevelsByTask(options.val);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            });
            th.Start();
            th.Join();
            return LstS;
        }
    }
}
