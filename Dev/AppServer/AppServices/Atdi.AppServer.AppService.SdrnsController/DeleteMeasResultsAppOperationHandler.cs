using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using Atdi.SDNRS.AppServer.BusManager;

namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class DeleteMeasResultsAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.DeleteMeasResultsAppOperation,
            DeleteMeasResultsAppOperationOptions,
            CommonOperationDataResult<int>
        >
    {
        public DeleteMeasResultsAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override CommonOperationDataResult<int> Handle(DeleteMeasResultsAppOperationOptions options, IAppOperationContext operationContext)
        {
            CommonOperationDataResult<int> cv_r = new CommonOperationDataResult<int>();
            KeyValuePair<string, object> ca_MeasTaskId = options.OtherArgs.Values.ToList().Find(t=>t.Key== "MeasTaskId");
            KeyValuePair<string, object> ca_StationId = options.OtherArgs.Values.ToList().Find(t => t.Key == "StationId");
            KeyValuePair<string, object> ca_SubMeasTaskId = options.OtherArgs.Values.ToList().Find(t => t.Key == "SubMeasTaskId");
            KeyValuePair<string, object> ca_SubMeasTaskStationId = options.OtherArgs.Values.ToList().Find(t => t.Key == "SubMeasTaskStationId");
            KeyValuePair<string, object> ca_N = options.OtherArgs.Values.ToList().Find(t => t.Key == "N");

            if ((ca_MeasTaskId.Value != null) 
                && (ca_StationId.Value != null)
                && (ca_SubMeasTaskId.Value != null)
                && (ca_SubMeasTaskStationId.Value != null)
                && (ca_N.Value != null)) {
                lock (GlobalInit.LST_MeasurementResults) {
                    MeasurementResults resd = GlobalInit.LST_MeasurementResults.Find(t => t.Id.MeasTaskId.Value == (int)ca_MeasTaskId.Value && t.StationMeasurements.StationId.Value == (int)ca_StationId.Value && t.Id.SubMeasTaskId == (int)ca_SubMeasTaskId.Value && t.Id.SubMeasTaskStationId == (int)ca_SubMeasTaskStationId.Value && (t.MeasurementsResults.ToList().Find(y=>y.Id.Value==(int)ca_N.Value)!=null));
                    if (resd!=null) {
                        ClassesDBGetResult resDb = new ClassesDBGetResult();
                        if (resd.Status=="O") {
                            GlobalInit.LST_MeasurementResults.RemoveAll(t => t.Id.MeasTaskId.Value == (int)ca_MeasTaskId.Value && t.StationMeasurements.StationId.Value == (int)ca_StationId.Value && t.Id.SubMeasTaskId == (int)ca_SubMeasTaskId.Value && t.Id.SubMeasTaskStationId == (int)ca_SubMeasTaskStationId.Value && (t.MeasurementsResults.ToList().Find(y => y.Id.Value == (int)ca_N.Value) != null));
                        }
                        if (resDb.DeleteResultFromDB(resd,"Z")) {
                            GlobalInit.LST_MeasurementResults.RemoveAll(t => t.Id.MeasTaskId.Value == (int)ca_MeasTaskId.Value && t.StationMeasurements.StationId.Value == (int)ca_StationId.Value && t.Id.SubMeasTaskId == (int)ca_SubMeasTaskId.Value && t.Id.SubMeasTaskStationId == (int)ca_SubMeasTaskStationId.Value && (t.MeasurementsResults.ToList().Find(y => y.Id.Value == (int)ca_N.Value) != null));
                            cv_r.State = CommonOperationState.Success;
                        }
                        else cv_r.State = CommonOperationState.Fault;
                    }
                    else cv_r.State = CommonOperationState.Fault;
                }
            }
            else if (ca_MeasTaskId.Value != null){
                lock (GlobalInit.LST_MeasurementResults) {
                    MeasurementResults resd = GlobalInit.LST_MeasurementResults.Find(t => t.Id.MeasTaskId.Value == (int)ca_MeasTaskId.Value);
                    resd.StationMeasurements = null;
                    if (resd != null) {
                        ClassesDBGetResult resDb = new ClassesDBGetResult();
                        if (resd.Status == "O") {
                            GlobalInit.LST_MeasurementResults.RemoveAll(t => t.Id.MeasTaskId.Value == (int)ca_MeasTaskId.Value);
                        }
                        if (resDb.DeleteResultFromDB(resd,"Z")) {
                            GlobalInit.LST_MeasurementResults.RemoveAll(t => t.Id.MeasTaskId.Value == (int)ca_MeasTaskId.Value);
                            cv_r.State = CommonOperationState.Success;
                        }
                        else cv_r.State = CommonOperationState.Fault;
                    }
                    else cv_r.State = CommonOperationState.Fault;
                }
            }
            Logger.Trace(this, options, operationContext);
            return  cv_r;
        }
    }
}
