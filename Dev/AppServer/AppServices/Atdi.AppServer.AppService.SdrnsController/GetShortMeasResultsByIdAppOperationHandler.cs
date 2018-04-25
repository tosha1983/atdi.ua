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
            Logger.Trace(this, options, operationContext);
            //lock (GlobalInit.LST_MeasurementResults)
            {
                if (options.MeasResultsId != null)
                {
                    if (options.MeasResultsId.MeasTaskId != null)
                    {
                        MeasurementResults msrt = GlobalInit.LST_MeasurementResults.Find(t => t.Id.MeasSdrResultsId == options.MeasResultsId.MeasSdrResultsId && t.Id.MeasTaskId.Value== options.MeasResultsId.MeasTaskId.Value && t.Id.SubMeasTaskId == options.MeasResultsId.SubMeasTaskId && t.Id.SubMeasTaskStationId == options.MeasResultsId.SubMeasTaskStationId);
                        if (msrt != null)
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
                        }
                    }
                }
            }
            return ShortMeas;
        }
    }

}