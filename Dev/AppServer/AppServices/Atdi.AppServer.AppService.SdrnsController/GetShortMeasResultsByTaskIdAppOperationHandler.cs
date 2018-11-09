using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetShortMeasResultsByTaskIdAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetShortMeasResultsByTaskIdAppOperation,
            GetShortMeasResultsByTaskIdAppOperationOptions,
            ShortMeasurementResults[]
        >
    {

        public GetShortMeasResultsByTaskIdAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }


        public override ShortMeasurementResults[] Handle(GetShortMeasResultsByTaskIdAppOperationOptions options, IAppOperationContext operationContext)
        {
            Logger.Trace(this, options, operationContext);
            ShortMeasurementResults[] ShortMeas =null;
            ClassesDBGetResult resDb = new ClassesDBGetResult(Logger);
            ClassConvertToSDRResults conv = new ClassConvertToSDRResults(Logger);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    List<MeasurementResults> LST_MeasurementResults = conv.ConvertTo_SDRObjects(resDb.ReadResultFromDBTask(options.TaskId.Value));
                    List<MeasurementResults> msrt = LST_MeasurementResults.FindAll(t => t.Id.MeasTaskId.Value == options.TaskId.Value);
                    if (msrt != null)
                    {
                        ShortMeas = new ShortMeasurementResults[msrt.Count];
                        for (int i=0; i< msrt.Count; i++)
                        {
                            ShortMeas[i] = new ShortMeasurementResults();
                            ShortMeasurementResults ShMsrt = new ShortMeasurementResults { DataRank = msrt[i].DataRank, Id = msrt[i].Id, Number = msrt[i].N !=null ? msrt[i].N.Value : -1, Status = msrt[i].Status, TimeMeas = msrt[i].TimeMeas, TypeMeasurements = msrt[i].TypeMeasurements };
                            if (msrt[i].LocationSensorMeasurement != null)
                            {
                                if (msrt[i].LocationSensorMeasurement.Count() > 0)
                                {
                                    ShMsrt.CurrentLat = msrt[i].LocationSensorMeasurement[msrt[i].LocationSensorMeasurement.Count() - 1].Lat;
                                    ShMsrt.CurrentLon = msrt[i].LocationSensorMeasurement[msrt[i].LocationSensorMeasurement.Count() - 1].Lon;
                                }
                            }
                            ShortMeas[i]=ShMsrt;
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