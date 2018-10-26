using System;
using System.Collections.Generic;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.BusManager;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;

//using Atdi.AppServer.AppService.SdrnsController.ConstraintParsers;

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetSOformMeasResultStationAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetSOformMeasResultStationAppOperation,
            GetSOformMeasResultStationAppOperationOptions,
            SOFrequency[]
        >
    {

        public GetSOformMeasResultStationAppOperationHandler(IAppServerContext serverContext, ILogger logger)
            : base(serverContext, logger)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override SOFrequency[] Handle(GetSOformMeasResultStationAppOperationOptions options, IAppOperationContext operationContext)
        {
            Logger.Trace(this, options, operationContext);
            AnaliticsUnit1 analiticsUnit1 = new AnaliticsUnit1(Logger);
            List<SOFrequency> LstS = new List<SOFrequency>();
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                   
                    LstS.AddRange(analiticsUnit1.CalcAppUnit2(options.val.Frequencies_MHz, options.val.BW_kHz, options.val.MeasResultID, options.val.LonMax, options.val.LonMin, options.val.LatMax, options.val.LatMin, options.val.TrLevel_dBm));
                    //analiticsUnit1.CalcAppUnit2(new List<double> { 2162.4, 2167.2 }, 4157, new List<int> { 1404 }, 31, 29, 51, 49, -56);
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
