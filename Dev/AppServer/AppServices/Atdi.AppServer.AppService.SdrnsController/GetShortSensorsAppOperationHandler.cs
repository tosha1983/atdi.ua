﻿using System;
using System.Collections.Generic;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.BusManager;

//using Atdi.AppServer.AppService.SdrnsController.ConstraintParsers;

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetShortSensorsAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetShortSensorsAppOperation,
            GetShortSensorsAppOperationOptions,
            ShortSensor[]
        >
    {

        public GetShortSensorsAppOperationHandler(IAppServerContext serverContext, ILogger logger)
            : base(serverContext, logger)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override ShortSensor[] Handle(GetShortSensorsAppOperationOptions options, IAppOperationContext operationContext)
        {
            Logger.Trace(this, options, operationContext);
            SensorListSDRNS senLst = new SensorListSDRNS(Logger);
            List<ShortSensor> LstS = new List<ShortSensor>();
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    LstS = senLst.CreateShortSensorList();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            });
            th.Start();
            th.Join();
            return LstS.ToArray();
        }
    }
}
