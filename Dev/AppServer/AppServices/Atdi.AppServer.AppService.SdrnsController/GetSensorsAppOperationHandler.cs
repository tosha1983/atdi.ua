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
using Atdi.SDNRS.AppServer;


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
            val = GlobalInit.SensorListSDRNS;
            return val.ToArray();
        }
    }
     
}

