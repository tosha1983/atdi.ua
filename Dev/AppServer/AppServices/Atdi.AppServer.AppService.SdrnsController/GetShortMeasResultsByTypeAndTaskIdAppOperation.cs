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
            ShortMeasurementResultsExtend[]
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
        public override ShortMeasurementResultsExtend[] Handle(GetShortMeasResultsByTypeAndTaskIdAppOperationOptions options, IAppOperationContext operationContext)
        {
            ShortMeasurementResultsExtend[] ShortMeas = null;
            ClassesDBGetResultOptimize resDb = new ClassesDBGetResultOptimize(Logger);
            ClassConvertToSDRResultsOptimize conv = new ClassConvertToSDRResultsOptimize(Logger);
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    var res = conv.ConvertMeasurementResultsExt(resDb.ReadlAllResultFromDBByIdTask(options.measurementType, options.TaskId));
                    if (res != null)
                    {
                        ShortMeas = new ShortMeasurementResultsExtend[res.Count];
                        for (int i = 0; i < res.Count; i++)
                        {
                            ShortMeas[i] = new ShortMeasurementResultsExtend();
                            ShortMeasurementResultsExtend ShMsrt = new ShortMeasurementResultsExtend { SensorName = res[i].Value, DataRank = res[i].Key.DataRank, Id = res[i].Key.Id, Number = res[i].Key.N != null ? res[i].Key.N.Value : -1, Status = res[i].Key.Status, TimeMeas = res[i].Key.TimeMeas, TypeMeasurements = res[i].Key.TypeMeasurements };
                            if (res[i].Key.LocationSensorMeasurement != null)
                            {
                                if (res[i].Key.LocationSensorMeasurement.Count() > 0)
                                {
                                    ShMsrt.CurrentLat = res[i].Key.LocationSensorMeasurement[res[i].Key.LocationSensorMeasurement.Count() - 1].Lat;
                                    ShMsrt.CurrentLon = res[i].Key.LocationSensorMeasurement[res[i].Key.LocationSensorMeasurement.Count() - 1].Lon;
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
