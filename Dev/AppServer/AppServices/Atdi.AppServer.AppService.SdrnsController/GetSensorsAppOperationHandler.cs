using System;
using System.Collections.Generic;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetSensorsAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetSensorsAppOperation,
            GetSensorsAppOperationOptions,
            Sensor[]
        >
    {

        public GetSensorsAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }

      
        public override Sensor[] Handle(GetSensorsAppOperationOptions options, IAppOperationContext operationContext)
        {
            Logger.Trace(this, options, operationContext);
            List<Sensor> val = new List<Sensor>();
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                try
                {
                    ClassDBGetSensor gsd = new ClassDBGetSensor(Logger);
                    val = gsd.LoadObjectAllSensor();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }

            });
            thread.Start();
            thread.Join();
            return val.ToArray();
        }
    }
     
}

