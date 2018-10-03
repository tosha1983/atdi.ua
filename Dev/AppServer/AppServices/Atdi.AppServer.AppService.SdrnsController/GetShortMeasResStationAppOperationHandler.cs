using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetShortMeasResStationAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetShortMeasResStationAppOperation,
            GetShortMeasResStationAppOperationOptions,
            ShortResultsMeasurementsStation[]
        >
    {

        public GetShortMeasResStationAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }


        public override ShortResultsMeasurementsStation[] Handle(GetShortMeasResStationAppOperationOptions options, IAppOperationContext operationContext)
        {
            ShortResultsMeasurementsStation[] ShortMeas = null;
            ClassesDBGetResult resDb = new ClassesDBGetResult(Logger);
            ClassConvertToSDRResults conv = new ClassConvertToSDRResults(Logger);
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    if (options.ResId != null)
                    {
                       
                           ShortMeas = conv.ConvertTo_ShortResultsMeasurementsStation(resDb.ReadShortResultResMeasStationsFromDB(options.ResId.Value));
                        
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            });
            th.Start();
            th.Join();
            return ShortMeas;
        }
    }

}