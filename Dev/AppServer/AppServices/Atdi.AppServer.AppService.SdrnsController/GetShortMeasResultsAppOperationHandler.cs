﻿using System;
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
    public class GetShortMeasResultsAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetShortMeasResultsAppOperation,
            GetShortMeasResultsAppOperationOptions,
            ShortMeasurementResults[]
        >
    {
        public GetShortMeasResultsAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override ShortMeasurementResults[] Handle(GetShortMeasResultsAppOperationOptions options, IAppOperationContext operationContext)
        {
            List<ShortMeasurementResults> ShortMeas = new List<ShortMeasurementResults>();
            ClassesDBGetResult resDb = new ClassesDBGetResult(Logger);
            ClassConvertToSDRResults conv = new ClassConvertToSDRResults(Logger);
            Logger.Trace(this, options, operationContext);
            System.Threading.Thread th = new System.Threading.Thread(() =>
            {
                List<MeasurementResults> LST_MeasurementResults = conv.ConvertTo_SDRObjects(resDb.ReadlAllResultFromDB()).ToList();
                foreach (MeasurementResults msrt in LST_MeasurementResults) {
                    ShortMeasurementResults ShMsrt = new ShortMeasurementResults { DataRank = msrt.DataRank,  Id = msrt.Id, Number = msrt.N.Value, Status = msrt.Status, TimeMeas = msrt.TimeMeas, TypeMeasurements = msrt.TypeMeasurements };
                    if (msrt.LocationSensorMeasurement!=null) {
                        if (msrt.LocationSensorMeasurement.Count()>0) {
                            ShMsrt.CurrentLat = msrt.LocationSensorMeasurement[msrt.LocationSensorMeasurement.Count() - 1].Lat;
                            ShMsrt.CurrentLon = msrt.LocationSensorMeasurement[msrt.LocationSensorMeasurement.Count() - 1].Lon;
                        }
                    }
                    ShortMeas.Add(ShMsrt);
                }
            
            });
            th.Start();
            th.IsBackground = true;
            th.Join();
            return ShortMeas.ToArray();
        }
    }
}
