using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetResMeasStationHeaderByResIdAppOperationHandle
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetResMeasStationHeaderByResIdAppOperation,
            GetShortMeasResStationAppOperationOptions,
            ResultsMeasurementsStationExtended[]
        >
    {

        public GetResMeasStationHeaderByResIdAppOperationHandle(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }


        public override ResultsMeasurementsStationExtended[] Handle(GetShortMeasResStationAppOperationOptions options, IAppOperationContext operationContext)
        {
            ResultsMeasurementsStationExtended[] resultsMeasurementsStation = null;
            ClassesDBGetResultOptimize resDb = new ClassesDBGetResultOptimize(Logger);
            ClassConvertToSDRResultsOptimize conv = new ClassConvertToSDRResultsOptimize(Logger);
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    if (options.ResId != null)
                    {
                        resultsMeasurementsStation = conv.ConvertStationHeaders(resDb.ReadResultStationHeaderByResId(options.ResId.Value)).ToArray();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            });
            th.Start();
            th.Join();
            return resultsMeasurementsStation.ToArray();
        }
    }

}