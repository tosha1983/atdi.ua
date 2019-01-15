using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetSensorPoligonPointAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetSensorPoligonPointAppOperation,
            GetShortMeasResStationAppOperationOptions,
            SensorPoligonPoint[]
        >
    {

        public GetSensorPoligonPointAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }


        public override SensorPoligonPoint[] Handle(GetShortMeasResStationAppOperationOptions options, IAppOperationContext operationContext)
        {
            SensorPoligonPoint[] sensorPoligonPoints = null;
            ClassesDBGetResult resDb = new ClassesDBGetResult(Logger);
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    if (options.ResId != null)
                    {
                        sensorPoligonPoints = resDb.GetSensorPoligonPoint(options.ResId.Value);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            });
            th.Start();
            th.Join();
            return sensorPoligonPoints;
        }
    }

}