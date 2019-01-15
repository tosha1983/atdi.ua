using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetShortMeasResultsByIdAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetShortMeasResultsByIdAppOperation,
            GetShortMeasResultsByIdAppOperationOptions,
            ShortMeasurementResults
        >
    {

        public GetShortMeasResultsByIdAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }


        public override ShortMeasurementResults Handle(GetShortMeasResultsByIdAppOperationOptions options, IAppOperationContext operationContext)
        {
            ShortMeasurementResults ShortMeas = new ShortMeasurementResults();
            ClassesDBGetResult resDb = new ClassesDBGetResult(Logger);
            ClassConvertToSDRResults conv = new ClassConvertToSDRResults(Logger);
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    if (options.MeasResultsId != null)
                    {
                        if (options.MeasResultsId != null)
                        {
                            List<MeasurementResults> msrtList = conv.ConvertTo_SDRObjects(resDb.ReadResultFromDB(options.MeasResultsId)).ToList();
                            if (msrtList != null)
                            {
                                foreach (MeasurementResults msrt in msrtList)
                                {
                                    ShortMeasurementResults ShMsrt = new ShortMeasurementResults { DataRank = msrt.DataRank, Id = msrt.Id, Number = msrt.N.Value, Status = msrt.Status, TimeMeas = msrt.TimeMeas, TypeMeasurements = msrt.TypeMeasurements };
                                    if (msrt.LocationSensorMeasurement != null)
                                    {
                                        if (msrt.LocationSensorMeasurement.Count() > 0)
                                        {
                                            ShMsrt.CurrentLat = msrt.LocationSensorMeasurement[msrt.LocationSensorMeasurement.Count() - 1].Lat;
                                            ShMsrt.CurrentLon = msrt.LocationSensorMeasurement[msrt.LocationSensorMeasurement.Count() - 1].Lon;
                                        }
                                    }
                                    ShortMeas = ShMsrt;
                                    break;
                                }
                            }
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