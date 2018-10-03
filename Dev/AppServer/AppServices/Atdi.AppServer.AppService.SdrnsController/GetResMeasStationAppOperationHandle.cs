using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetResMeasStationAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetResMeasStationAppOperation,
            GetResMeasStationAppOperationOptions,
            ResultsMeasurementsStation[]
        >
    {

        public GetResMeasStationAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }


        public override ResultsMeasurementsStation[] Handle(GetResMeasStationAppOperationOptions options, IAppOperationContext operationContext)
        {
            ResultsMeasurementsStation[] resultsMeasurementsStation = null;
            ClassesDBGetResult resDb = new ClassesDBGetResult(Logger);
            ClassConvertToSDRResults conv = new ClassConvertToSDRResults(Logger);
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    if ((options.ResId != null) && (options.StationId!=null))
                    {
                        resultsMeasurementsStation = conv.ConvertTo_ResultsMeasurementsStation(resDb.ReadResultResMeasStationsFromDB(options.ResId.Value, options.StationId.Value));

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