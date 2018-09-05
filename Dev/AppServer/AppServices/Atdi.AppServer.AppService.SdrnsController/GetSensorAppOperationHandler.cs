using System;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
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
                try
                { 
                 sens = gsd.LoadObjectSensor(options.SensorId.Value);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            });
            thread.Start();
            thread.Join();
            return sens;
        }
    }
     
}

