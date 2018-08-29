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
            List<ShortMeasurementResults> ShortMeas = new List<ShortMeasurementResults>();
            ClassesDBGetResult resDb = new ClassesDBGetResult(Logger);
            ClassConvertToSDRResults conv = new ClassConvertToSDRResults(Logger);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                try
                {
                    //List<MeasurementResults> LST_MeasurementResults = GlobalInit.blockingCollectionMeasurementResults.ToList().FindAll(t => t.Id.MeasTaskId.Value == options.TaskId.Value);
                    List<MeasurementResults> LST_MeasurementResults = conv.ConvertTo_SDRObjects(resDb.ReadResultFromDBTask(options.TaskId.Value)).ToList();
                    List<MeasurementResults> msrt = LST_MeasurementResults.FindAll(t => t.Id.MeasTaskId.Value == options.TaskId.Value);
                    if (msrt != null)
                    {
                        foreach (MeasurementResults rs in msrt)
                        {
                            ShortMeasurementResults ShMsrt = new ShortMeasurementResults { DataRank = rs.DataRank, Id = rs.Id, Number = rs.N.Value, Status = rs.Status, TimeMeas = rs.TimeMeas, TypeMeasurements = rs.TypeMeasurements };
                            if (rs.LocationSensorMeasurement != null)
                            {
                                if (rs.LocationSensorMeasurement.Count() > 0)
                                {
                                    ShMsrt.CurrentLat = rs.LocationSensorMeasurement[rs.LocationSensorMeasurement.Count() - 1].Lat;
                                    ShMsrt.CurrentLon = rs.LocationSensorMeasurement[rs.LocationSensorMeasurement.Count() - 1].Lon;
                                }
                            }
                            ShortMeas.Add(ShMsrt);
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
            return ShortMeas.ToArray();
        }
    }

}