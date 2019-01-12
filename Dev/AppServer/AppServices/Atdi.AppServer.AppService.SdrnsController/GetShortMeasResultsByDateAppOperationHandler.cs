using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;


namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetShortMeasResultsByDateAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetShortMeasResultsByDatesAppOperation,
            GetShortMeasResultsByDateAppOperationOptions,
            ShortMeasurementResultsExtend[]
        >
    {
        public GetShortMeasResultsByDateAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override ShortMeasurementResultsExtend[] Handle(GetShortMeasResultsByDateAppOperationOptions options, IAppOperationContext operationContext)
        {
            List<ShortMeasurementResultsExtend> ShortMeas = new List<ShortMeasurementResultsExtend>();
            ClassesDBGetResult resDb = new ClassesDBGetResult(Logger);
            ClassConvertToSDRResults conv = new ClassConvertToSDRResults(Logger);
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {

                    List<KeyValuePair<MeasurementResults, string>> LST_MeasurementResults = conv.ConvertTo_SDRObjectsExt(resDb.ReadlAllResultFromDB(options.options.Start.Value, options.options.End.Value)).ToList();
                    foreach (var msrt in LST_MeasurementResults)
                    {
                        ShortMeasurementResultsExtend ShMsrt = new ShortMeasurementResultsExtend { DataRank = msrt.Key.DataRank, Id = msrt.Key.Id, Number = msrt.Key.N.HasValue ? msrt.Key.N.Value : -1, Status = msrt.Key.Status, TimeMeas = msrt.Key.TimeMeas, TypeMeasurements = msrt.Key.TypeMeasurements, SensorName = msrt.Value };
                        if (msrt.Key.LocationSensorMeasurement != null)
                        {
                            if (msrt.Key.LocationSensorMeasurement.Count() > 0)
                            {
                                ShMsrt.CurrentLat = msrt.Key.LocationSensorMeasurement[msrt.Key.LocationSensorMeasurement.Count() - 1].Lat;
                                ShMsrt.CurrentLon = msrt.Key.LocationSensorMeasurement[msrt.Key.LocationSensorMeasurement.Count() - 1].Lon;
                            }
                        }
                        ShortMeas.Add(ShMsrt);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            });
            th.Start();
            th.Join();
            return ShortMeas.ToArray();
        }
    }
}
