using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;


namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetShortMeasResultsByTypeAndTaskIdAppOperation
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetShortMeasResultsByTypeAndTaskIdAppOperation,
            GetShortMeasResultsByTypeAndTaskIdAppOperationOptions,
            ShortMeasurementResults[]
        >
    {
        public GetShortMeasResultsByTypeAndTaskIdAppOperation(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override ShortMeasurementResults[] Handle(GetShortMeasResultsByTypeAndTaskIdAppOperationOptions options, IAppOperationContext operationContext)
        {
           ShortMeasurementResults[] ShortMeas = null;
            MeasurementResults[] res = null;
            ClassesDBGetResultOptimize resDb = new ClassesDBGetResultOptimize(Logger);
            ClassConvertToSDRResultsOptimize conv = new ClassConvertToSDRResultsOptimize(Logger);
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    res = conv.ConvertMeasurementResults(resDb.ReadlAllResultFromDBByIdTask(options.measurementType, options.TaskId)).ToArray();
                    if (res != null)
                    {
                        ShortMeas = new ShortMeasurementResults[res.Length];
                        for (int i = 0; i < res.Length; i++)
                        {
                            ShortMeas[i] = new ShortMeasurementResults();
                            ShortMeasurementResults ShMsrt = new ShortMeasurementResults { DataRank = res[i].DataRank, Id = res[i].Id, Number = res[i].N != null ? res[i].N.Value : -1, Status = res[i].Status, TimeMeas = res[i].TimeMeas, TypeMeasurements = res[i].TypeMeasurements };
                            if (res[i].LocationSensorMeasurement != null)
                            {
                                if (res[i].LocationSensorMeasurement.Count() > 0)
                                {
                                    ShMsrt.CurrentLat = res[i].LocationSensorMeasurement[res[i].LocationSensorMeasurement.Count() - 1].Lat;
                                    ShMsrt.CurrentLon = res[i].LocationSensorMeasurement[res[i].LocationSensorMeasurement.Count() - 1].Lon;
                                }
                            }
                            ShortMeas[i] = ShMsrt;
                        }
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
