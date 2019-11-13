using Atdi.Contracts.Api.DataBus;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.DataConstraint;
using Atdi.Contracts.Sdrn.Server;
using MD = Atdi.DataModels.Sdrns.Server.Entities;

namespace Atdi.AppUnits.Sdrn.AggregationServer.PrimaryHandlers.Handlers
{
    /// <summary>
    /// Загрузка из БД AggregationServer сведений о задачах
    /// </summary>
    public sealed class SaveMeasTask
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;


        public SaveMeasTask(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public long SetAggregationMeasTaskId(long valueIdmeasSubTaskSta, long subTaskSensorMasterId)
        {
            var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
            var builderUpdateLinkSensor = this._dataLayer.GetBuilder<MD.ILinkSubTaskSensorMasterId>().Insert();
            builderUpdateLinkSensor.SetValue(c => c.SubtaskSensorMasterId, subTaskSensorMasterId);
            builderUpdateLinkSensor.SetValue(c => c.SUBTASK_SENSOR.Id, valueIdmeasSubTaskSta);
            var linkAggregationSensor_PKId = queryExecuter.Execute<MD.ILinkSubTaskSensorMasterId_PK>(builderUpdateLinkSensor);
            return linkAggregationSensor_PKId.Id;
        }
    }
}
