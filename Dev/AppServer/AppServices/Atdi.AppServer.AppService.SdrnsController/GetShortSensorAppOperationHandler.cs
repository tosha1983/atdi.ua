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

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetShortSensorAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetShortSensorAppOperation,
            GetShortSensorAppOperationOptions,
            ShortSensor
        >
    {

       
        public GetShortSensorAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }


        public override ShortSensor Handle(GetShortSensorAppOperationOptions options, IAppOperationContext operationContext)
        {
            ShortSensor LstS = new ShortSensor();
            SensorListSDRNS senLst = new SensorListSDRNS(Logger);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    LstS = senLst.CreateShortSensorListBySensorId(options.SensorId.Value);
                    Logger.Trace(this, options, operationContext);
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