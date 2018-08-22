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
                if (options.MeasResultsId != null)
                {
                    if (options.MeasResultsId.MeasTaskId != null)
                    {
                        List<MeasurementResults> LST_MeasurementResults = conv.ConvertTo_SDRObjects(resDb.ReadResultFromDBTask(options.MeasResultsId.MeasTaskId.Value)).ToList();
                        MeasurementResults msrt = LST_MeasurementResults.Find(t => t.Id.MeasSdrResultsId == options.MeasResultsId.MeasSdrResultsId && t.Id.MeasTaskId.Value== options.MeasResultsId.MeasTaskId.Value && t.Id.SubMeasTaskId == options.MeasResultsId.SubMeasTaskId && t.Id.SubMeasTaskStationId == options.MeasResultsId.SubMeasTaskStationId);
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
            });
            th.Start();
            th.IsBackground = true;
            th.Join();
            return ShortMeas;
        }
    }

}