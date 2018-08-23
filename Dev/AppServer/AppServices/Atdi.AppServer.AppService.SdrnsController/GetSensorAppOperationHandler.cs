using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.BusManager;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;


namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetSensorAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetSensorAppOperation,
            GetSensorAppOperationOptions,
            Sensor
        >
    {

        public GetSensorAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }


        public override Sensor Handle(GetSensorAppOperationOptions options, IAppOperationContext operationContext)
        {
            Logger.Trace(this, options, operationContext);
            ClassDBGetSensor gsd = new ClassDBGetSensor(Logger);
            Sensor sens = null;
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                 sens = gsd.LoadObjectSensor(options.SensorId.Value);
            });
            thread.Start();
            thread.Join();
            return sens;
        }
    }
     
}

