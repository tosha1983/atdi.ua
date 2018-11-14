using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetResMeasStationByIdAppOperationHandle
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetResMeasStationByIdAppOperation,
            GetResMeasStationByIdAppOperationOptions,
            ResultsMeasurementsStation
        >
    {

        public GetResMeasStationByIdAppOperationHandle(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }


        public override ResultsMeasurementsStation Handle(GetResMeasStationByIdAppOperationOptions options, IAppOperationContext operationContext)
        {
            ResultsMeasurementsStation resultsMeasurementsStation = null;
            ClassesDBGetResultOptimize resDb = new ClassesDBGetResultOptimize(Logger);
            ClassConvertToSDRResultsOptimize conv = new ClassConvertToSDRResultsOptimize(Logger);
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    if (options.StationId != null)
                    {
                        resultsMeasurementsStation = conv.ConvertTo_ResultsMeasurementsOneStation(resDb.ReadResultResMeasStation(options.StationId.Value));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            });
            th.Start();
            th.Join();
            return resultsMeasurementsStation;
        }
    }

}